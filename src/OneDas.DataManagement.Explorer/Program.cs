using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using OneDas.DataManagement.Explorer.Core;
using Serilog.Extensions.Logging;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Explorer
{
    public class Program
    {
        #region Fields

        private static ILoggerFactory _loggerFactory;

        #endregion

        #region Properties

        public static string OptionsFilePath { get; private set; }

        public static OneDasExplorerOptions Options { get; private set; }

        public static OneDasDatabaseManager DatabaseManager { get; private set; }

        #endregion

        #region Methods

        public static async Task<int> Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            // check interactivity
            var isWindowsService = args.Contains("--non-interactive");

            // paths
            var appdataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OneDAS", "Explorer");
            Directory.CreateDirectory(appdataFolderPath);

            var logFolderPath = Path.Combine(appdataFolderPath, "LOGS");
            Directory.CreateDirectory(logFolderPath);

            // configure logging
            _loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddFile(Path.Combine(logFolderPath, "OneDasExplorer-{Date}.txt"), outputTemplate: OneDasConstants.FileLoggerTemplate);
            });

            // load configuration
            Program.OptionsFilePath = Path.Combine(appdataFolderPath, "settings.json");

            if (File.Exists(Program.OptionsFilePath))
            {
                Program.Options = OneDasExplorerOptions.Load(Program.OptionsFilePath);
            }
            else
            {
                Program.Options = new OneDasExplorerOptions();
                Program.Options.Save(Program.OptionsFilePath);
            }

            // load and update database
            var logger = _loggerFactory.CreateLogger("OneDAS Explorer");
            Program.DatabaseManager = new OneDasDatabaseManager(logger, _loggerFactory);

            // service vs. interactive
            if (isWindowsService)
                await Program
                    .CreateHostBuilder(Environment.CurrentDirectory, logFolderPath)
                    .UseWindowsService()
                    .Build()
                    .RunAsync();
            else
                await Program
                    .CreateHostBuilder(Environment.CurrentDirectory, logFolderPath)
                    .Build()
                    .RunAsync();

            return 0;
        }

        public static IHostBuilder CreateHostBuilder(string currentDirectory, string logFolderPath) => 
            Host.CreateDefaultBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();

                    logging.AddConsole();
                    logging.AddFilter<ConsoleLoggerProvider>("Microsoft", LogLevel.None);

                    logging.AddFile(Path.Combine(logFolderPath, "OneDasExplorer-{Date}.txt"), outputTemplate: OneDasConstants.FileLoggerTemplate);
                    logging.AddFilter<SerilogLoggerProvider>("Microsoft", LogLevel.None);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls(Program.Options.AspBaseUrl);
                    webBuilder.UseContentRoot(currentDirectory);
                    webBuilder.SuppressStatusMessages(true);
                });

        #endregion
    }
}
