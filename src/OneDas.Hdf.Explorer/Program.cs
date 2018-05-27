using HDF.PInvoke;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using OneDas.Hdf.Core;
using OneDas.Hdf.IO;
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
        private static Dictionary<string, string> _campaignDescriptionSet;
        private static HdfExplorerOptions _options;
        private static IConfiguration _configuration;

        public static void Main(string[] args)
        {
            bool isUserInteractive;
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

            Program.UpdateCampaignInfoSet();

            if (isUserInteractive)
            {
                Program.CreateWebHost().Run();
            }
            else
            {
                Program.CreateWebHost().RunAsService();
            }
        }

        public static List<CampaignInfo> CampaignInfoSet
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

        public static Dictionary<string, string> CampaignDescriptionSet
        {
            get
            {
                lock (_lock)
                {
                    return _campaignDescriptionSet;
                }
            }
            private set
            {
                _campaignDescriptionSet = value;
            }
        }

        public static void UpdateCampaignInfoSet()
        {
            long vdsFileId = -1;
            long vdsMetaFileId = -1;
            long groupId = -1;

            lock (_lock)
            {
                try
                {
                    if (File.Exists(_options.VdsFilePath))
                    {
                        vdsFileId = H5F.open(_options.VdsFilePath, H5F.ACC_RDONLY);
                        vdsMetaFileId = H5F.open(_options.VdsMetaFilePath, H5F.ACC_RDONLY);

                        Program.CampaignInfoSet = GeneralHelper.GetCampaignInfoSet(vdsFileId, false);
                    }
                    else
                    {
                        Program.CampaignInfoSet = new List<CampaignInfo>();
                    }

                    Program.CampaignDescriptionSet = Program.CampaignInfoSet.ToDictionary(campaignInfo => campaignInfo.Name, campaignInfo =>
                    {
                        if (IOHelper.CheckLinkExists(vdsMetaFileId, campaignInfo.Name))
                        {
                            try
                            {
                                groupId = H5G.open(vdsMetaFileId, campaignInfo.Name);

                                if (H5A.exists(groupId, "description") > 0)
                                {
                                    return IOHelper.ReadAttribute<string>(groupId, "description").First();
                                }
                            }
                            finally
                            {
                                if (H5I.is_valid(groupId) > 0) { H5G.close(groupId); }
                            }
                        }

                        return "no description available";
                    });
                }
                finally
                {
                    if (H5I.is_valid(vdsFileId) > 0) { H5F.close(vdsFileId); }
                    if (H5I.is_valid(vdsMetaFileId) > 0) { H5F.close(vdsMetaFileId); }
                }
            }
        }

        private static IWebHost CreateWebHost()
        {
            IWebHost webHost;

            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")))
            {
                Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            }

            webHost = new WebHostBuilder()
                .ConfigureServices(services => services.Configure<HdfExplorerOptions>(_configuration))
                .UseKestrel()
                .UseUrls(_options.AspBaseUrl)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .SuppressStatusMessages(true)
                .Build();

            return webHost;
        }
    }
}
