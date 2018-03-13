using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OneDas.Hdf.Explorer.Web;
using System.IO;

namespace OneDas.Hdf.Explorer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSignalR().AddJsonProtocol(options =>
            {
                options.PayloadSerializerSettings = new JsonSerializerSettings();
            });

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddConsole();
                loggingBuilder.AddFilter((provider, source, logLevel) => !source.StartsWith("Microsoft."));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IOptions<HdfExplorerOptions> options)
        {
            ILogger logger;

            logger = loggerFactory.CreateLogger("HDF Explorer");

            logger.LogInformation($"Listening on: { options.Value.AspBaseUrl }");

            if (string.IsNullOrWhiteSpace(options.Value.DataBaseFolderPath))
            {
                logger.LogError($"No database directory path has been configured.");
            }
            else if (!Directory.Exists(options.Value.DataBaseFolderPath))
            {
                logger.LogWarning($"Configured database directory path { options.Value.DataBaseFolderPath } does not exist.");
            }
            else
            {
                logger.LogInformation($"Database directory found at path: { options.Value.DataBaseFolderPath }");
            }

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
                routes.MapHub<Broadcaster>("/broadcaster");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}