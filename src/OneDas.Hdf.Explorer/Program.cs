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
using System.Text.Json;

namespace OneDas.Hdf.Explorer
{
    public class Program
    {
        #region Fields

        private static object _lock;
        private static HdfExplorerOptions _options;
        private static IConfiguration _configuration;
        private static Dictionary<IDataSource, IReadOnlyList<CampaignInfo>> _dataSourceToCampaignMap;
        private static Dictionary<CampaignInfo, CampaignMetaInfo> _campaignToCampaignMetaMap;

        #endregion

        #region Methods

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

        public static void UpdateCampaignInfos()
        {
            lock (_lock)
            {
                _dataSourceToCampaignMap = new Dictionary<IDataSource, IReadOnlyList<CampaignInfo>>();
                _campaignToCampaignMetaMap = new Dictionary<CampaignInfo, CampaignMetaInfo>();

                var dataSources = new List<IDataSource>()
                {
                    new HdfDataSource(_options.DataBaseFolderPath)
                };

                foreach (var dataSource in dataSources)
                {
                    var campaignInfos = dataSource.GetCampaignInfos();

                    foreach (var campaignInfo in campaignInfos)
                    {
                        _campaignToCampaignMetaMap[campaignInfo] = Program.LoadCampaignMeta(campaignInfo);
                    }

                    _dataSourceToCampaignMap[dataSource] = campaignInfos;
                }
            }
        }

        public static void SaveCampaignMeta(CampaignMetaInfo campaignMeta)
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, "DB_META", $"{campaignMeta.Name}.json");

            if (!Program.GetCampaigns().Any(campaign => campaign.Name == campaignMeta.Name))
                throw new InvalidOperationException($"The campaign '{campaignMeta.Name}' does not exist within the database.");

            campaignMeta.Purge();

            var jsonString = JsonSerializer.Serialize(campaignMeta);
            File.WriteAllText(filePath, jsonString);
        }

        public static List<CampaignInfo> GetCampaigns()
        {
            lock (_lock)
            {
                return _dataSourceToCampaignMap.SelectMany(x => x.Value).ToList();
            }
        }

        public static IDataSource GetDataSource(string campaignName)
        {
            lock (_lock)
            {
                var dataSource = _dataSourceToCampaignMap.Where(x => x.Value.Any(campaign => campaign.Name == campaignName)).Select(x => x.Key).FirstOrDefault();

                if (dataSource == null)
                    throw new KeyNotFoundException("The requested campaign could not be found.");

                return dataSource;
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

        private static CampaignMetaInfo LoadCampaignMeta(CampaignInfo campaign)
        {
            CampaignMetaInfo campaignMeta;

            var filePath = Path.Combine(Environment.CurrentDirectory, "DB_META", $"{campaign.Name}.json");

            if (!File.Exists(filePath))
                campaignMeta = new CampaignMetaInfo(campaign.Name);
            else
            {
                var jsonString = File.ReadAllText(filePath);
                campaignMeta = JsonSerializer.Deserialize<CampaignMetaInfo>(jsonString);
            }

            return campaignMeta;
        }

        #endregion
    }
}
