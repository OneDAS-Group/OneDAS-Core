using HDF.PInvoke;
using OneDas.DataManagement.Database;
using OneDas.DataManagement.Extensibility;
using OneDas.DataManagement.Hdf;
using OneDas.Extensibility;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Extensions
{
    [ExtensionIdentification("OneDas.HDF", "OneDAS HDF", "Provides access to databases with OneDAS HDF files.", "", "")]
    public class HdfDataReader : DataReaderExtensionBase
    {
        #region Fields

        private List<CampaignInfo> _campaigns;
        private string _filePath;
        private long _fileId = -1;

        #endregion

        #region Constructors

        public HdfDataReader(string rootPath) : base(rootPath)
        {
            _filePath = Path.Combine(this.RootPath, "VDS.h5");
        }

        #endregion

        #region Methods

        public override List<string> GetCampaignNames()
        {
            this.EnsureOpened();

            _campaigns = GeneralHelper.GetCampaigns(_fileId).Select(hdfCampaign => hdfCampaign.ToCampaign()).ToList();

            this.SwitchLocation(() =>
            {
                foreach (var campaign in _campaigns)
                {
                    GeneralHelper.UpdateCampaignStartAndEnd(_fileId, campaign, maxProbingCount: 20);
                }
            });

            return _campaigns.Select(campaign => campaign.Id).ToList();
        }

        public override CampaignInfo GetCampaign(string campaignName)
        {
            return _campaigns.First(campaign => campaign.Id == campaignName);
        }

        public override (T[] Dataset, byte[] StatusSet) ReadSingle<T>(DatasetInfo dataset, DateTime begin, DateTime end)
        {
            this.EnsureOpened();

            T[] data = null;
            byte[] statusSet = null;

            var samplesPerDay = dataset.GetSampleRate().SamplesPerDay;
            (var start, var block) = GeneralHelper.GetStartAndBlock(begin, end, samplesPerDay);

            this.SwitchLocation(() =>
            {
                var datasetPath = dataset.GetPath();
                data = IOHelper.ReadDataset<T>(_fileId, datasetPath, start, block);

                if (H5L.exists(_fileId, datasetPath + "_status") > 0)
                    statusSet = IOHelper.ReadDataset(_fileId, datasetPath + "_status", start, block).Cast<byte>().ToArray();
            });

            return (data, statusSet);
        }

        public override void Dispose()
        {
            if (H5I.is_valid(_fileId) > 0) { H5F.close(_fileId); }
        }

        protected override double GetDataAvailability(string campaignName, DateTime day)
        {
            if (!_campaigns.Any(campaign => campaign.Id == campaignName))
                throw new Exception($"The campaign '{campaignName}' could not be found.");

            this.EnsureOpened();

            // epoch & hyperslab
            var epochStart = new DateTime(2000, 01, 01);
            var epochEnd = new DateTime(2030, 01, 01);

            if (!(epochStart <= day && day <= epochEnd))
                throw new Exception("requirement >> epochStart <= day && day <= epochEnd << is not matched");

            var samplesPerDay = new SampleRateContainer("is_chunk_completed_set").SamplesPerDay;

            var start = (ulong)Math.Floor((day - epochStart).TotalDays * samplesPerDay);
            var block = samplesPerDay;

            // get data
            var result = 0.0;

            this.SwitchLocation(() =>
            {
                var data = IOHelper.ReadDataset<byte>(_fileId, $"{campaignName}/is_chunk_completed_set", start, block).Select(value => (int)value).ToArray();
                result = data.Sum() / 1440.0;
            });

            return result;
        }

        private void EnsureOpened()
        {
            if (!(H5I.is_valid(_fileId) > 0))
                _fileId = H5F.open(_filePath, H5F.ACC_RDONLY);
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
