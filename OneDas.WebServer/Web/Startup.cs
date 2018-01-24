using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OneDas.Engine.Core;
using OneDas.Engine.Serialization;

namespace OneDas.WebServer.Web
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
            // Add framework services.
            services.AddMvc();

            services.AddSignalR(hubOptions =>
            {
                var settings = new JsonSerializerSettings()
                {
                    Converters = { new OneDasConverter() }
                };

                hubOptions.JsonSerializerSettings = settings;
            });

            services.AddResponseCompression();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IOptions<WebServerOptions> options)
        {
            WebServerOptions webServerOptions;

            webServerOptions = options.Value;

            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseSignalR(routes =>
            {
                routes.MapHub<WebClientHub>(webServerOptions.WebClientHubName);
                routes.MapHub<ConsoleHub>(webServerOptions.ConsoleHubName);
            });

            app.UseMvc(routes =>
            {
                //routes.MapRoute(
                //    name: "default",
                //    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "api",
                    template: "{controller}/{action}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{*url}",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
