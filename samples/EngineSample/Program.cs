using Microsoft.Extensions.DependencyInjection;
using OneDas.Engine.Core;
using OneDas.Infrastructure;
using System;

namespace EngineSample
{
    class Program
    {
        /* improvements:
         * - settings.ChannelHubSet.Add(new ChannelHub()); // improve ChannelHub constructor
         * - remove Trace.WriteLine
         * - is retryCount counted correctly?
         * - reduce OneDasEngine dependencies
         */

        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            ConfigureServices(services);

            var provider = services.BuildServiceProvider();
            var engine = provider.GetRequiredService<OneDasEngine>();
            var settings = new OneDasProjectSettings("OneDAS", "Engine", "Example");

            engine.ActivateProject(settings, 1);
            engine.Start();

            Console.ReadKey();
        }

        static void ConfigureServices(IServiceCollection services)
        {
            services.AddOneDas();
        }
    }
}
