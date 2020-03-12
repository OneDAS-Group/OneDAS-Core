using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneDas.DataManagement.BlazorExplorer.Core;
using OneDas.DataManagement.BlazorExplorer.Hubs;
using OneDas.DataManagement.BlazorExplorer.ViewModels;
using System;
using System.IO;
using System.Security.Claims;

namespace OneDas.DataManagement.BlazorExplorer
{
    public class Startup
    {
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

            // authorization
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdmin", policy => policy.RequireClaim("IsAdmin", "true"));
            });

            // signalr
            services.AddSignalR()
                .AddMessagePackProtocol();

            // authorization

            // custom
            services.AddHttpContextAccessor();
            services.AddScoped<AppStateViewModel>();
            services.AddScoped<SettingsViewModel>();
            services.AddScoped<DataService>();
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

            // ...
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            
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

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapHub<DataHub>("datahub");
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
