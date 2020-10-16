using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OneDas.DataManagement.Explorer.Core;
using OneDas.DataManagement.Explorer.Hubs;
using OneDas.DataManagement.Explorer.ViewModels;
using System;
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

            // signalr
            services.AddSignalR(options => options.EnableDetailedErrors = true)
                .AddMessagePackProtocol();

            // custom
            services.AddHttpContextAccessor();
            services.AddScoped<AppStateViewModel>();
            services.AddScoped<SettingsViewModel>();
            services.AddScoped<DataService>();
            services.AddScoped<JwtService<IdentityUser>>();
            services.AddSingleton(Program.DatabaseManager);
            services.AddSingleton(Program.Options);
            services.AddSingleton<OneDasExplorerStateManager>();
            services.AddSingleton<OneDasExplorerUserManager>();
        }

        public void Configure(IApplicationBuilder app,
                              IWebHostEnvironment env,
                              ILoggerFactory loggerFactory,
                              OneDasExplorerStateManager stateManager, // stateManager is requested to create an instance and let the timers start
                              OneDasExplorerOptions options)
        {
            // logger
            var logger = loggerFactory.CreateLogger("OneDAS Explorer");
            logger.LogInformation($"Listening on: { options.AspBaseUrl }");

            // ...
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
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

            // routing (for REST API)
            app.UseRouting();

            // default authentication
            app.UseAuthentication();

            // custom authentication (to also authenticate via JWT bearer)
            app.Use(async (context, next) =>
            {
                if (!context.User.Identity.IsAuthenticated)
                {
                    var principal = await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);

                    if (principal.Succeeded)
                        context.User = principal.Principal;
                }

                await next();
            });

            // authorization
            app.UseAuthorization();

            // endpoints
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapHub<DataHub>("datahub");
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        #endregion
    }
}
