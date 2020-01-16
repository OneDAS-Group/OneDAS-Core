using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using OneDas.Database;
using OneDas.Hdf.Explorer.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OneDas.Hdf.Explorer
{
    public class Program
    {
        private static object _lock;
        private static List<CampaignInfo> _campaignInfoSet;
        private static HdfExplorerOptions _options;
        private static IConfiguration _configuration;
        private static List<IDataSource> _dataSources;

        public static void Main(string[] args)
        {
            bool isUserInteractive;

            string currentDirectory;
            string configurationDirectoryPath;
            string configurationFileName;

            IConfigurationBuilder configurationBuilder;

            isUserInteractive = !args.Contains("--non-interactive");
            _lock = new object();

            // configuration
            configurationDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OneDAS", "HDF Explorer");
            configurationFileName = "settings.json";

            Directory.CreateDirectory(configurationDirectoryPath);

            configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile(new PhysicalFileProvider(configurationDirectoryPath), path: configurationFileName, optional: true, reloadOnChange: true);

            _configuration = configurationBuilder.Build();
            _options = _configuration.Get<HdfExplorerOptions>();

            if (_options == null)
            {
                _options = new HdfExplorerOptions();
                _configuration.Bind(_options);
            }

            _options.Save(configurationDirectoryPath);

            // databases
            _dataSources = new List<IDataSource>()
            {
                new HdfDataSource(_options.DataBaseFolderPath)
            };

            // campaign info
            Program.UpdateCampaignInfos();

            // change current directory to database location
            currentDirectory = Environment.CurrentDirectory;

            if (Directory.Exists(_options.DataBaseFolderPath))
                Environment.CurrentDirectory = _options.DataBaseFolderPath;

            // service vs. interactive
            if (isUserInteractive)
                Program.CreateWebHost(currentDirectory).Run();
            else
                Program.CreateWebHost(currentDirectory).RunAsService();
        }

        public static List<CampaignInfo> CampaignInfos
        {
            get
            {
                lock (_lock)
                {
                    return _campaignInfoSet;
                }
            }
            private set
            {
                _campaignInfoSet = value;
            }
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

        public static void UpdateCampaignInfos()
        {
            lock (_lock)
            {
                Program.CampaignInfos = _dataSources.SelectMany(source => source.GetCampaignInfos()).ToList();
            }
        }
    }
}
