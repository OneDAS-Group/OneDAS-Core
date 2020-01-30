using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Explorer
{
    public class Program
    {
        #region Fields

        private static HdfExplorerOptions _options;
        private static IConfiguration _configuration;
        private static ILoggerFactory _loggerFactory;

        #endregion

        #region Properties

        public static OneDasDatabaseManager DatabaseManager { get; private set; }

        #endregion

        #region Methods

        public static async Task<int> Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            // check interactivity
            var isUserInteractive = !args.Contains("--non-interactive");

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
            _options = _configuration.Get<HdfExplorerOptions>();

            if (_options == null)
            {
                _options = new HdfExplorerOptions();
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
            Program.DatabaseManager.Update();

            // service vs. interactive
            if (isUserInteractive)
                await Program.CreateWebHost(currentDirectory).RunAsync();
            else
                Program.CreateWebHost(currentDirectory).RunAsService();

            return 0;
        }

        private static IWebHost CreateWebHost(string currentDirectory)
        {
            if (!Directory.Exists(Path.Combine(currentDirectory, "wwwroot")))
                currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            var webHost = new WebHostBuilder()
                .ConfigureServices(services => services.Configure<HdfExplorerOptions>(_configuration))
                .UseKestrel()
                .UseUrls(_options.AspBaseUrl)
                .UseContentRoot(currentDirectory)
                .UseStartup<Startup>()
                .SuppressStatusMessages(true)
                .Build();

            return webHost;
        }

        #endregion
    }
}
