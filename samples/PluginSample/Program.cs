using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OneDas.Infrastructure;
using OneDas.Plugin;
using OneDas.Plugin.DataGateway.Example;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace EngineSample
{
    class Program
    {
        /* improvements:
        * - make reading big endian values easier
        * - FrameRateDivider should not be set as it is not used in this example
        */

        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            ConfigureServices(services);

            var provider = services.BuildServiceProvider();
            var pluginProvider = provider.GetRequiredService<IPluginProvider>();

            pluginProvider.Add(typeof(ExampleGateway));

            var modbusTcpModel = new ExampleSettings();
            modbusTcpModel.ModuleSet.Add(new OneDasModule(OneDasDataType.FLOAT32, DataDirection.Input, Endianness.LittleEndian, 10));
            modbusTcpModel.Validate();

            Console.CursorVisible = false;

            Task.Run(() =>
            {
                using (var exampleGateway = pluginProvider.BuildLogic<ExampleGateway>(modbusTcpModel))
                {
                    exampleGateway.Configure();

                    while (true)
                    {
                        exampleGateway.UpdateIo(DateTime.Now);

                        var values = MemoryMarshal.Cast<byte, float>(exampleGateway.GetInputBuffer());

                        Console.Clear();

                        for (int i = 0; i < 10; i++)
                        {
                            Console.WriteLine(String.Format(CultureInfo.InvariantCulture, "{0,6:F2}", values[i]));
                        }

                        Thread.Sleep(1000);
                    }
                }
            });

            Console.ReadKey(true);
        }

        static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IPluginProvider, PluginProvider>();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.SetMinimumLevel(LogLevel.Debug);
                loggingBuilder.AddConsole();
            });
        }
    }
}
