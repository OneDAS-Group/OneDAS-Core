using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OneDas.DataManagement.Explorer.Core;
using OneDas.DataManagement.Explorer.Hubs;
using OneDas.DataManagement.Explorer.ViewModels;
using System;
using System.IO;
using System.Security.Claims;
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
            services.AddSingleton<OneDasExplorerStateManager>();
        }

        public void Configure(IApplicationBuilder app,
                              IWebHostEnvironment env,
                              ILoggerFactory loggerFactory,
                              ApplicationDbContext userDB,
                              UserManager<IdentityUser> userManager,
                              IOptions<OneDasExplorerOptions> options)
        {
            // logger
            var logger = loggerFactory.CreateLogger("OneDAS Explorer");
            logger.LogInformation($"Listening on: { options.Value.AspBaseUrl }");

            // database
            if (userDB.Database.EnsureCreated())
                logger.LogInformation($"Database initialized.");

            // ensure there is a root user
            if (userManager.FindByNameAsync("root@root.org").Result == null)
            {
                var user = new IdentityUser()
                {
                    UserName = "root@root.org",
                };

                var defaultPassword = "#root0/User1";
                var result = userManager.CreateAsync(user, defaultPassword).Result;

                if (result.Succeeded)
                {
                    var claim = new Claim("IsAdmin", "true");
                    userManager.AddClaimAsync(user, claim).Wait();
                }
            }

            // ensure there is a test user
            if (userManager.FindByNameAsync("test@root.org").Result == null)
            {
                var user = new IdentityUser()
                {
                    UserName = "test@root.org",
                };

                var defaultPassword = "#test0/User1";
                var result = userManager.CreateAsync(user, defaultPassword).Result;
            }

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
                FileProvider = new PhysicalFileProvider(Path.Combine(Environment.CurrentDirectory, "ATTACHMENTS")),
                RequestPath = "/attachments"
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Environment.CurrentDirectory, "PRESETS")),
                RequestPath = "/presets"
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Environment.CurrentDirectory, "SUPPORT", "EXPORT")),
                RequestPath = "/export"
            });

            // routing
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
                endpoints.MapBlazorHub();
                endpoints.MapHub<DataHub>("datahub");
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        #endregion
    }
}
