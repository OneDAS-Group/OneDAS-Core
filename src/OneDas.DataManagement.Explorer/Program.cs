using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
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

        private static OneDasExplorerOptions _options;
        private static IConfiguration _configuration;
        private static ILoggerFactory _loggerFactory;

        #endregion

        #region Properties

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
            var configurationFileName = "settings.json";
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile(new PhysicalFileProvider(appdataFolderPath), path: configurationFileName, optional: true, reloadOnChange: true);

            _configuration = configurationBuilder.Build();
            _options = _configuration.Get<OneDasExplorerOptions>();

            if (_options == null)
            {
                _options = new OneDasExplorerOptions();
                _configuration.Bind(_options);
                _options.Save(appdataFolderPath);
            }

            // change current directory to database location
            var currentDirectory = Environment.CurrentDirectory;

            if (Directory.Exists(_options.DataBaseFolderPath))
                Environment.CurrentDirectory = _options.DataBaseFolderPath;

            if (!OneDasUtilities.ValidateDatabaseFolderPath(Environment.CurrentDirectory, out var message))
            {
                Console.WriteLine(message);
                return 1;
            }

            // load and update database
            var logger = _loggerFactory.CreateLogger("OneDAS Explorer");
            Program.DatabaseManager = new OneDasDatabaseManager(logger, _loggerFactory);

            // service vs. interactive
            if (isWindowsService)
                await Program.CreateHostBuilder(currentDirectory, logFolderPath).UseWindowsService().Build().RunAsync();
            else
                await Program.CreateHostBuilder(currentDirectory, logFolderPath).Build().RunAsync();

            return 0;
        }

        public static IHostBuilder CreateHostBuilder(string currentDirectory, string logFolderPath) => 
            Host.CreateDefaultBuilder()
                .ConfigureServices(services => services.Configure<OneDasExplorerOptions>(_configuration))
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
                    webBuilder.UseUrls(_options.AspBaseUrl);
                    webBuilder.UseContentRoot(currentDirectory);
                    webBuilder.SuppressStatusMessages(true);
                });

        #endregion
    }
}
