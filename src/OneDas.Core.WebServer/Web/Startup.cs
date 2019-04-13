using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OneDas.Core.Serialization;

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
            services.AddMvc().AddRazorPagesOptions(options => {
                options.RootDirectory = "/Web/Pages";
                options.Conventions.AddPageRoute("/Index", "{*url}");
            });

            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            }).AddNewtonsoftJsonProtocol(options =>
            {
                var settings = new JsonSerializerSettings()
                {
                    Converters = { new OneDasConverter() }
                };

                options.PayloadSerializerSettings = settings;
            });

            services.AddResponseCompression();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<WebServerOptions> options)
        {
            WebServerOptions webServerOptions;

            webServerOptions = options.Value;

            app.UseResponseCompression();

            if (env.EnvironmentName == "Development")
            {
                app.UseDeveloperExceptionPage();
                app.UseBlazorDebugging();
            }

            app.UseStaticFiles();

            app.UseSignalR(routes =>
            {
                routes.MapHub<WebClientHub>("/" + webServerOptions.WebClientHubName);
                routes.MapHub<ConsoleHub>("/" + webServerOptions.ConsoleHubName);
            });

            app.UseBlazor<OneDas.Core.WebClient.Startup>();
        }
    }
}
