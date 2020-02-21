using HDF.PInvoke;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneDas.DataManagement.Explorer.Core;
using OneDas.DataManagement.Explorer.Web;
using System;
using System.IO;
using System.Text.Json;

namespace OneDas.DataManagement.Explorer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .AddRazorPagesOptions(options =>
                {
                    options.RootDirectory = "/Web/Pages";
                });

            services
                .AddSignalR(options => options.EnableDetailedErrors = true)
                .AddJsonProtocol(options => options.PayloadSerializerOptions = new JsonSerializerOptions() { PropertyNamingPolicy = null });

            services.AddSingleton<OneDasExplorerStateManager>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IOptions<HdfExplorerOptions> options)
        {
            var logger = loggerFactory.CreateLogger("OneDAS Explorer");

            this.Validate(logger, options);

            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapHub<Broadcaster>("/broadcaster");
            });
        }

        private void Validate(ILogger logger, IOptions<HdfExplorerOptions> options)
        {
            // check thread safety of HDF library
            var isLibraryThreadSafe = 0U;

            H5.is_library_threadsafe(ref isLibraryThreadSafe);

            if (isLibraryThreadSafe <= 0)
                logger.LogError("The libary 'hdf5.dll' is not thread safe.");

            // asp base URL
            logger.LogInformation($"Listening on: { options.Value.AspBaseUrl }");
        }
    }
}