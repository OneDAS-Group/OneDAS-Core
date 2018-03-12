using HDF.PInvoke;
using Microsoft.AspNetCore.Hosting;
using OneDas.Hdf.Core;
using OneDas.Hdf.IO;
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

        public static void Main(string[] args)
        {
            Program.BaseDirectoryPath = @"M:\DATABASE";

            _lock = new object();

            Directory.CreateDirectory(Path.Combine(Program.BaseDirectoryPath, "SUPPORT", "EXPORT"));
            Directory.CreateDirectory(Path.Combine(Program.BaseDirectoryPath, "SUPPORT", "LOGS", "HDF Explorer"));

            Program.VdsFilePath = Path.Combine(Program.BaseDirectoryPath, "VDS.h5");
            Program.VdsMetaFilePath = Path.Combine(Program.BaseDirectoryPath, "VDS_META.h5");

            Program.UpdateCampaignInfoSet();
            Program.BuildWebHost(args).Run();
        }

        public static string BaseDirectoryPath { get; private set; }
        public static string VdsFilePath { get; private set; }
        public static string VdsMetaFilePath { get; private set; }

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
                    if (File.Exists(Program.VdsFilePath))
                    {
                        vdsFileId = H5F.open(Program.VdsFilePath, H5F.ACC_RDONLY);
                        vdsMetaFileId = H5F.open(Program.VdsMetaFilePath, H5F.ACC_RDONLY);

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
                            groupId = H5G.open(vdsMetaFileId, campaignInfo.Name);

                            if (H5A.exists(groupId, "description") > 0)
                            {
                                return IOHelper.ReadAttribute<string>(groupId, "description").First();
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

        public static IWebHost BuildWebHost(string[] args) =>
            new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://0.0.0.0:32769")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();
    }
}
