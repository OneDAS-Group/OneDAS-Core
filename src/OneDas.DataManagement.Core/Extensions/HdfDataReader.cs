using HDF.PInvoke;
using OneDas.DataManagement.Database;
using OneDas.DataManagement.Extensibility;
using OneDas.DataManagement.Hdf;
using OneDas.Extensibility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OneDas.DataManagement.Extensions
{
    [ExtensionIdentification("OneDas.HDF", "OneDAS HDF", "Provides access to databases with OneDAS HDF files.", "", "")]
    public class HdfDataReader : DataReaderExtensionBase
    {
        #region Fields

        private string _filePath;
        private long _fileId = -1;
        private static object _lock;

        #endregion

        #region Constructors

        static HdfDataReader()
        {
            _lock = new object();
        }

        public HdfDataReader(string rootPath) : base(rootPath)
        {
            _filePath = Path.Combine(this.RootPath, "VDS.h5");
            _fileId = H5F.open(_filePath, H5F.ACC_RDONLY);
        }

        #endregion

        #region Methods

        public override (T[] Dataset, byte[] StatusSet) ReadSingle<T>(DatasetInfo dataset, DateTime begin, DateTime end)
        {

            T[] data = null;
            byte[] statusSet = null;

            var samplesPerDay = dataset.GetSampleRate().SamplesPerDay;
            (var start, var block) = GeneralHelper.GetStartAndBlock(begin, end, samplesPerDay);

            lock (_lock)
            {
                this.SwitchLocation(() =>
                {
                    var datasetPath = dataset.GetPath();
                    data = IOHelper.ReadDataset<T>(_fileId, datasetPath, start, block);

                    if (H5L.exists(_fileId, datasetPath + "_status") > 0)
                        statusSet = IOHelper.ReadDataset(_fileId, datasetPath + "_status", start, block).Cast<byte>().ToArray();
                });
            }

            return (data, statusSet);
        }

        public override void Dispose()
        {
            if (H5I.is_valid(_fileId) > 0) { H5F.close(_fileId); }
        }

        protected override List<CampaignInfo> LoadCampaigns()
        {
            var campaigns = GeneralHelper.GetCampaigns(_fileId).Select(hdfCampaign => hdfCampaign.ToCampaign()).ToList();

            lock (_lock)
            {
                this.SwitchLocation(() =>
                {
                    foreach (var campaign in campaigns)
                    {
                        GeneralHelper.UpdateCampaignStartAndEnd(_fileId, campaign, maxProbingCount: 20);
                    }
                });
            }            

            return campaigns;
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
            lock (_lock)
            {
                this.SwitchLocation(() =>
                {
                    var data = IOHelper.ReadDataset<byte>(_fileId, $"{campaignId}/is_chunk_completed_set", start, block).Select(value => (int)value).ToArray();
                    result = data.Sum() / 1440.0;
                });
            }

            return result;
        }

        private void SwitchLocation(Action action)
        {
            var currentLocation = Environment.CurrentDirectory;
            Environment.CurrentDirectory = this.RootPath;

            try
            {
                action.Invoke();
            }
            finally
            {
                Environment.CurrentDirectory = currentLocation;
            }
        }

        #endregion
    }
}
