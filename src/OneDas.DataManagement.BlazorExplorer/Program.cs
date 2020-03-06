using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Serilog.Extensions.Logging;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OneDas.DataManagement.BlazorExplorer
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

            // configure logging
            var serviceProvider = new ServiceCollection().AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddFile(Path.Combine(Environment.CurrentDirectory, "SUPPORT", "LOGS", "HdfExplorer-{Date}.txt"), outputTemplate: OneDasConstants.FileLoggerTemplate);
            }).BuildServiceProvider();

            _loggerFactory = serviceProvider.GetService<ILoggerFactory>();

            // load configuration
            var configurationDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OneDAS", "Explorer");
            var configurationFileName = "settings.json";

            Directory.CreateDirectory(configurationDirectoryPath);

            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile(new PhysicalFileProvider(configurationDirectoryPath), path: configurationFileName, optional: true, reloadOnChange: true);

            _configuration = configurationBuilder.Build();
            _options = _configuration.Get<OneDasExplorerOptions>();

            if (_options == null)
            {
                _options = new OneDasExplorerOptions();
                _configuration.Bind(_options);
                _options.Save(configurationDirectoryPath);
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
            Program.DatabaseManager = new OneDasDatabaseManager();

            // service vs. interactive
            if (isWindowsService)
                await Program.CreateHostBuilder(currentDirectory).UseWindowsService().Build().RunAsync();
            else
                await Program.CreateHostBuilder(currentDirectory).Build().RunAsync();

            return 0;
        }

        public static IHostBuilder CreateHostBuilder(string currentDirectory) => 
            Host.CreateDefaultBuilder()
                .ConfigureServices(services => services.Configure<OneDasExplorerOptions>(_configuration))
                .ConfigureLogging(loggingBuilder =>
                {
                    loggingBuilder.ClearProviders();

                    loggingBuilder.AddConsole();
                    loggingBuilder.AddFilter<ConsoleLoggerProvider>("Microsoft", LogLevel.None);

                    loggingBuilder.AddFile(Path.Combine(Environment.CurrentDirectory, "SUPPORT", "LOGS", "OneDasExplorer-{Date}.txt"), outputTemplate: OneDasConstants.FileLoggerTemplate);
                    loggingBuilder.AddFilter<SerilogLoggerProvider>("Microsoft", LogLevel.None);
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
