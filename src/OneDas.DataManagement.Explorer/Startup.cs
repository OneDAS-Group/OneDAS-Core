using GraphQL.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OneDas.DataManagement.Explorer.API;
using OneDas.DataManagement.Explorer.Core;
using OneDas.DataManagement.Explorer.Services;
using OneDas.DataManagement.Explorer.ViewModels;
using System;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Explorer
{
    public class Startup
    {
        #region Properties

        public static SymmetricSecurityKey SecurityKey { get; } = new SymmetricSecurityKey(Guid.NewGuid().ToByteArray());

        #endregion

        #region Methods

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // database
            services.AddDbContext<ApplicationDbContext>();

            // forwarded headers
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.All;
#warning replace this with proper external configuration
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            // identity (customize: https://docs.microsoft.com/en-us/aspnet/core/security/authentication/customize-identity-model?view=aspnetcore-3.1)
            services.AddDefaultIdentity<IdentityUser>()
                .AddEntityFrameworkStores<ApplicationDbContext>();            

            // blazor
            services.AddRazorPages();
            services.AddServerSideBlazor();

            // authentication
            services.AddAuthentication(options =>
            {
                // Identity made Cookie authentication the default.
                // Custom authentication needed (see app.Use(...) below).
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        LifetimeValidator = (before, expires, token, parameters) => expires > DateTime.UtcNow,
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateActor = false,
                        ValidateLifetime = true,
                        IssuerSigningKey = Startup.SecurityKey
                    };

                    options.Events = new JwtBearerEvents()
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            if (!string.IsNullOrEmpty(accessToken) && context.Request.Headers["Upgrade"] == "websocket")
                                context.Token = accessToken;

                            return Task.CompletedTask;
                        }
                    };
                });

            // authorization
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdmin", policy => policy.RequireClaim("IsAdmin", "true"));
            });

            // swagger
            services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            services.AddSwaggerDocument(config =>
            {
                config.Title = "OneDAS Explorer REST API";
                config.Version = "v1";
                config.Description = "Explore resources and get their data.";
                //config.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT Token"));
                //config.AddSecurity("JWT Token", Enumerable.Empty<string>(),
                //    new OpenApiSecurityScheme()
                //    {
                //        Type = OpenApiSecuritySchemeType.ApiKey,
                //        Name = "Authorization",
                //        In = OpenApiSecurityApiKeyLocation.Header,
                //        Description = "Copy this into the value field: Bearer {token}"
                //    }
                //);
            });

            // graphql
            services
                .AddSingleton<ProjectSchema>()
                .AddGraphQL((options, provider) =>
                {
                    options.EnableMetrics = false;

                    var logger = provider.GetRequiredService<ILogger<Startup>>();
                    options.UnhandledExceptionDelegate = ctx
                        => logger.LogError($"{ctx.OriginalException.Message} occured");
                })
                .AddErrorInfoProvider(opt => opt.ExposeExceptionStackTrace = true)
                .AddSystemTextJson(deserializerSettings => { }, serializerSettings => { })
                .AddGraphTypes(typeof(ProjectSchema));

            // custom
            services.AddHttpContextAccessor();
            services.AddScoped<UserIdService>();
            services.AddScoped<AppStateViewModel>();
            services.AddScoped<SettingsViewModel>();
            services.AddScoped<JwtService<IdentityUser>>();
            services.AddTransient<DataService>();
            services.AddTransient<AggregationService>();
            services.AddSingleton(Program.DatabaseManager);
            services.AddSingleton(Program.Options);
            services.AddSingleton<StateManager>();
            services.AddSingleton<JobService<ExportJob>>();
            services.AddSingleton<JobService<AggregationJob>>();
            services.AddSingleton<UserManager>();
            services.AddSingleton<FileAccessManager>();
        }

        public void Configure(IApplicationBuilder app,
                              IWebHostEnvironment env,
                              StateManager stateManager, // needs to be called to initialize the database
                              OneDasExplorerOptions options)
        {
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-5.0

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseForwardedHeaders();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseForwardedHeaders();
            }

            // static files
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new LazyPhysicalFileProvider(options, "ATTACHMENTS"),
                RequestPath = "/attachments",
                ServeUnknownFileTypes = true
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new LazyPhysicalFileProvider(options, "PRESETS"),
                RequestPath = "/presets"
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new LazyPhysicalFileProvider(options, "EXPORT"),
                RequestPath = "/export"
            });

            // swagger
            app.UseOpenApi();
            app.UseSwaggerUi3();

            // graphql
            app.UseGraphQL<ProjectSchema>("/graphql");
            app.UseGraphQLPlayground();

            // routing (for REST API)
            app.UseRouting();

            // default authentication
            app.UseAuthentication();

            // custom authentication (to also authenticate via JWT bearer)
            app.Use(async (context, next) =>
            {
                bool terminate = false;

                if (!context.User.Identity.IsAuthenticated)
                {
                    var authorizationHeader = context.Request.Headers["Authorization"];
                    
                    if (authorizationHeader.Any(header => header.StartsWith("Bearer")))
                    {
                        var result = await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);

                        if (result.Succeeded)
                        {
                            context.User = result.Principal;
                        }
                        else
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                            var errorCode = result.Failure.Message.Split(':', count: 2).FirstOrDefault();

                            var message = errorCode switch
                            {
                                "IDX10230" => "Lifetime validation failed.",
                                "IDX10503" => "Signature validation failed.",
                                _ => "The bearer token could not be validated."
                            };

                            var bytes = Encoding.UTF8.GetBytes(message);
                            await context.Response.Body.WriteAsync(bytes);
                            terminate = true;
                        }
                    }
                }

                if (!terminate)
                    await next();
            });

            // authorization
            app.UseAuthorization();

            // endpoints
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        #endregion
    }
}
