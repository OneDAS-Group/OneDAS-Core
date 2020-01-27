using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using OneDas.Data;
using OneDas.DataManagement;
using OneDas.DataManagement.Extensibility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OneDas.Hdf.Explorer
{
    public class Program
    {
        #region Fields

        private static OneDasDatabase _database;
        private static HdfExplorerOptions _options;
        private static IConfiguration _configuration;
        private static ILoggerFactory _loggerFactory;
        private static Dictionary<string, DataReaderExtensionBase> _idToDataReaderMap;

        #endregion

        #region Properties

        public static OneDasDatabase Database { get; private set; }

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
            var configurationDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OneDAS", "HDF Explorer");
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

            // load database
            Program.LoadDatabase();

            // service vs. interactive
            if (isUserInteractive)
                await Program.CreateWebHost(currentDirectory).RunAsync();
            else
                Program.CreateWebHost(currentDirectory).RunAsService();

            return 0;
        }

        public static void LoadDatabase()
        {
            (Program.Database, _idToDataReaderMap) = DatabaseUtilities.Load();
        }

        public static DataReaderExtensionBase GetDataReader(string campaignName)
        {
            var container = _database.CampaignContainers.FirstOrDefault(container => container.Name == campaignName);

            if (container == null)
                throw new KeyNotFoundException("The requested campaign could not be found.");

            if (!_database.RootPathToDataReaderIdMap.TryGetValue(container.RootPath, out var id))
                throw new KeyNotFoundException("The requested root path could not be found.");

            if (!_idToDataReaderMap.TryGetValue(id, out var dataReader))
                throw new KeyNotFoundException("The requested data reader could not be found.");

            return dataReader;
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
