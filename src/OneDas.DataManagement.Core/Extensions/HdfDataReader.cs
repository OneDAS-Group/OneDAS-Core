using HDF.PInvoke;
using IwesOneDas;
using Microsoft.Extensions.Logging;
using OneDas.DataManagement.Database;
using OneDas.DataManagement.Extensibility;
using OneDas.DataManagement.Hdf;
using OneDas.Extensibility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace OneDas.DataManagement.Extensions
{
    [ExtensionIdentification("OneDas.HDF", "OneDAS HDF", "Provides access to databases with OneDAS HDF files.", "", "")]
    public class HdfDataReader : DataReaderExtensionBase
    {
        #region Constructors

        public HdfDataReader(string rootPath, ILogger logger) : base(rootPath, logger)
        {
            //
        }

        #endregion

        #region Methods

        public override (T[] Dataset, byte[] StatusSet) ReadSingle<T>(DatasetInfo dataset, DateTime begin, DateTime end)
        {

            T[] data = null;
            byte[] statusSet = null;

            var samplesPerDay = dataset.GetSampleRate().SamplesPerDay;
            (var start, var block) = GeneralHelper.GetStartAndBlock(begin, end, samplesPerDay);

            //lock (_lock)
            //{
            //    this.SwitchLocation(() =>
            //    {
                    //var datasetPath = dataset.GetPath();
                    //data = IOHelper.ReadDataset<T>(_fileId, datasetPath, start, block);

                    //if (H5L.exists(_fileId, datasetPath + "_status") > 0)
                    //    statusSet = IOHelper.ReadDataset(_fileId, datasetPath + "_status", start, block).Cast<byte>().ToArray();
            //    });
            //}

            return (data, statusSet);
        }

        protected override List<CampaignInfo> LoadCampaigns()
        {
            // (0) load versioning file
            var versioningFilePath = Path.Combine(this.RootPath, "versioning.json");

            var versioning = File.Exists(versioningFilePath)
                ? HdfVersioning.Load(versioningFilePath)
                : new HdfVersioning();

            // (1) find beginning of database
            var dataFolderPath = Path.Combine(this.RootPath, "DATA");
            Directory.CreateDirectory(dataFolderPath);

            var firstMonthString = Path.GetFileName(Directory.EnumerateDirectories(dataFolderPath).FirstOrDefault());

            if (firstMonthString == null)
                return new List<CampaignInfo>();

            var firstMonth = DateTime.ParseExact(firstMonthString, "yyyy-MM", CultureInfo.InvariantCulture);

            // (2) for each month
            var now = DateTime.UtcNow;
            var months = ((now.Year - firstMonth.Year) * 12) + now.Month - firstMonth.Month + 1;
            var currentMonth = firstMonth;

            var cacheFolderPath = Path.Combine(this.RootPath, "CACHE");
            Directory.CreateDirectory(cacheFolderPath);

            for (int i = 0; i < months; i++)
            {
                var currentDataFolderPath = Path.Combine(dataFolderPath, currentMonth.ToString("yyyy-MM"));

                // (3) find available campaign names by scanning file names
                var campaignNames = this.FindCampaignNames(Path.Combine(dataFolderPath, currentMonth.ToString("yyyy-MM")));

                // (4) find corresponding cache file
                var cacheFilePath = Path.Combine(cacheFolderPath, $"{currentMonth.ToString("yyyy-MM")}.json");

                List<CampaignInfo> campaigns;

                // (5.a) cache file exists
                if (File.Exists(cacheFilePath))
                {
                    campaigns = JsonSerializerHelper.Deserialize<List<CampaignInfo>>(cacheFilePath);
                    campaigns.ForEach(campaign => campaign.Initialize());

                    foreach (var campaignName in campaignNames)
                    {
                        var campaign = campaigns.FirstOrDefault(campaign => campaign.Id == campaignName);

                        // campaign is in cache
                        if (campaign != null)
                        {
                            // cache is outdated
                            if (this.IsCacheOutdated(campaignName, currentDataFolderPath, versioning))
                                campaign = this.ScanFiles(campaignName, currentDataFolderPath);
                        }
                        // campaign is not in cache
                        else
                        {
                            campaign = this.ScanFiles(campaignName, currentDataFolderPath);
                            campaigns.Add(campaign);
                        }
                    }
                }
                // (5.b) cache file does not exist
                else
                {
                    campaigns = campaignNames.Select(campaignName =>
                    {
                        var campaign = this.ScanFiles(campaignName, currentDataFolderPath);
                        return campaign;
                    }).ToList();
                }

                // (6) save cache file
                JsonSerializerHelper.Serialize(campaigns, cacheFilePath);

                currentMonth.AddMonths(1);
            }

            // (7) save versioning file
            JsonSerializerHelper.Serialize(versioning, versioningFilePath);

            // (8) merge cache files
#warning not yet implemented
#warning Update LastScan missing
            return new List<CampaignInfo>();
        }

        protected override double GetDataAvailability(string campaignId, DateTime day)
        {
            if (!this.Campaigns.Any(campaign => campaign.Id == campaignId))
                throw new Exception($"The campaign '{campaignId}' could not be found.");

            // epoch & hyperslab
            var epochStart = new DateTime(2000, 01, 01);
            var epochEnd = new DateTime(2030, 01, 01);

            if (!(epochStart <= day && day <= epochEnd))
                throw new Exception("requirement >> epochStart <= day && day <= epochEnd << is not matched");

            var samplesPerDay = 1440UL;
            var start = (ulong)Math.Floor((day - epochStart).TotalDays * samplesPerDay);
            var block = samplesPerDay;

            // get data
            var result = 0.0;

#warning Switching the location is no good idea. It breaks the webserver. Locks is required since this method is called with Parallel.For() which causes the current location to be set incorrectly.
            //lock (_lock)
            //{
            //    this.SwitchLocation(() =>
            //    {
                    //var data = IOHelper.ReadDataset<byte>(_fileId, $"{campaignId}/is_chunk_completed_set", start, block).Select(value => (int)value).ToArray();
                    //result = data.Sum() / 1440.0;
            //    });
            //}

            return result;
        }

        private List<string> FindCampaignNames(string dataFolderPath)
        {
            var distinctFiles = Directory
                .EnumerateFiles(dataFolderPath, "*.h5")
                .Select(filePath =>
                {
                    var fileName = Path.GetFileName(filePath);
                    return fileName.Substring(0, fileName.Length - 24);
                })
                .Distinct();

            return distinctFiles.Select(distinctFile =>
            {
                var filePath = Directory.EnumerateFiles(dataFolderPath, $"{distinctFile}*.h5").First();
                var campaignName = GeneralHelper.GetCampaignNameFromFile(filePath);
                return campaignName;
            }).Distinct().ToList();
        }

        private bool IsCacheOutdated(string campaignName, string dataFolderPath, HdfVersioning versioning)
        {
            var lastFile = Directory.EnumerateFiles($"{campaignName}*.h5").Last();
            var dateString = lastFile.Substring(lastFile.Length - 24, lastFile.Length);
            var date = DateTime.ParseExact(dateString, "yyyy-MM-ddTHH-mm-ssZ", CultureInfo.InvariantCulture);

            return date > versioning.ScannedUntil;
        }

        private CampaignInfo ScanFiles(string campaignId, string dataFolderPath)
        {
            var files = Directory.EnumerateFiles(dataFolderPath, $"{campaignId.Replace('/', '_').TrimStart('_')}*.h5");
            var campaign = new CampaignInfo(campaignId);

            foreach (var file in files)
            {
                var fileId = H5F.open(file, H5F.ACC_RDONLY);

                try
                {
                    if (H5I.is_valid(fileId) > 0)
                    {
                        var newCampaign = GeneralHelper.GetCampaign(fileId, campaignId);
#warning make sure merge is working correctly for this use case
#warning ToCampaign: Replace HdFCampaignInfo by normal CampaignInfo
                        campaign.Merge(newCampaign.ToCampaign());
                    }
                }
                finally
                {
                    if (H5I.is_valid(fileId) > 0) { H5F.close(fileId); }
                }
            }

            return campaign;
        }

        public override void Dispose()
        {
            //
#warning make virtual
        }

        #endregion
    }
}
