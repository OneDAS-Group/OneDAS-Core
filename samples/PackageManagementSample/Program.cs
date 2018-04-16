using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OneDas.Core.Engine;
using OneDas.Core.ProjectManagement;
using System;

namespace PackageManagementSample
{
    class Program
    {
        /* improvements:
         * - complete sample
         */

        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            ConfigureServices(services);

            var provider = services.BuildServiceProvider();
            var engine = provider.GetRequiredService<OneDasEngine>();
            var settings = new OneDasProjectSettings("OneDAS", "Engine", "Example");

            engine.ActivateProject(settings);
            engine.Start();

            Console.ReadKey();
        }

        static void ConfigureServices(IServiceCollection services)
        {
            services.AddOneDas();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.SetMinimumLevel(LogLevel.Debug);
                loggingBuilder.AddConsole();
            });  
        }
    }
}
