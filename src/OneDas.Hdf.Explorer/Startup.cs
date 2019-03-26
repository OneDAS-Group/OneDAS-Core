using HDF.PInvoke;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OneDas.Hdf.Explorer.Core;
using OneDas.Hdf.Explorer.Web;
using System.IO;

namespace OneDas.Hdf.Explorer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddRazorPagesOptions(options =>
                {
                    options.RootDirectory = "/Web/Pages";
                });

            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            }).AddJsonProtocol(options =>
            {
                options.PayloadSerializerSettings = new JsonSerializerSettings();
            });

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddConsole();
                loggingBuilder.AddFilter((provider, source, logLevel) => !source.StartsWith("Microsoft."));
            });

            services.AddSingleton<HdfExplorerStateManager>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IOptions<HdfExplorerOptions> options)
        {
            ILogger logger;

            logger = loggerFactory.CreateLogger("HDF Explorer");

            this.Validate(logger, options);

            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();

            app.UseSignalR(routes =>
            {
                routes.MapHub<Broadcaster>("/broadcaster");
            });

            app.UseMvc();
        }

        private void Validate(ILogger logger, IOptions<HdfExplorerOptions> options)
        {
            uint isLibraryThreadSafe;

            // check thread safety of HDF library
            isLibraryThreadSafe = 0;

            H5.is_library_threadsafe(ref isLibraryThreadSafe);

            if (isLibraryThreadSafe <= 0)
            {
                logger.LogError("The libary 'hdf5.dll' is not thread safe.");
            }

            // asp base URL
            logger.LogInformation($"Listening on: { options.Value.AspBaseUrl }");

            // check if database directory is configured and existing
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
        }
    }
}