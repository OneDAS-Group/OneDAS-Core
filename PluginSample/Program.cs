using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OneDas.Infrastructure;
using OneDas.Plugin;
using OneDas.Plugin.DataGateway.ModbusTcp;
using System;
using System.IO;
using System.Threading;

namespace EngineSample
{
    class Program
    {
        /* improvements:
        * - modbusTcpGateway.InputBuffer should be a Span<T> pointing to native memory
        * - make reading big endian values easier
        * - FrameRateDivider should not be set as it is not used in this example
        * - make example stoppable
        */

        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            ConfigureServices(services);

            var provider = services.BuildServiceProvider();
            var pluginProvider = provider.GetRequiredService<IPluginProvider>();

            pluginProvider.Add(typeof(ModbusTcpGateway));

            var modbusTcpModel = new ModbusTcpModel();
            modbusTcpModel.ModuleSet.Add(new ModbusTcpModule(0, ModbusTcpObjectTypeEnum.HoldingRegister, OneDasDataType.FLOAT32, DataDirection.Input, Endianness.BigEndian, 10));
            modbusTcpModel.FrameRateDivider = 4;
            modbusTcpModel.RemoteIpAddressString = "192.168.0.2";
            modbusTcpModel.Validate();

            var modbusTcpGateway = pluginProvider.BuildLogic<ModbusTcpGateway>(modbusTcpModel);
            modbusTcpGateway.Configure();

            while (true)
            {
                modbusTcpGateway.UpdateIo(DateTime.Now);

                for (int i = 0; i < 10; i++)
                {
                    var value = new Span<byte>(modbusTcpGateway.InputBuffer, i * 4, 4);
                    var singleValue = value.ToArray();

                    Array.Reverse(singleValue);
                    Console.Write(BitConverter.ToSingle(singleValue, 0) + "; ");
                }

                Console.WriteLine();
                Console.WriteLine();

                Thread.Sleep(1000);
            }

            Console.ReadKey();
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
