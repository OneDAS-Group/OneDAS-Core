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

            _campaigns = GeneralHelper.GetCampaigns(_fileId).ToList();

            this.SwitchLocation(() =>
            {
                foreach (var campaign in _campaigns)
                {
                    GeneralHelper.UpdateCampaignStartAndEnd(_fileId, campaign, maxProbingCount: 20);
                }
            });

            return _campaigns.Select(campaign => campaign.Name).ToList();
        }

        public override CampaignInfo GetCampaign(string campaignName)
        {
            return _campaigns.First(campaign => campaign.Name == campaignName);
        }

        public override bool IsDataOfDayAvailable(string campaignName, DateTime dateTime)
        {
            var folderPath = Path.Combine(this.RootPath, "DATA", dateTime.ToString("yyyy-MM"));
            var campaignNameFile = campaignName.TrimStart('/').Replace('/', '_');
            var fileNamePattern = $"{campaignNameFile}_V*_{dateTime.ToString("yyyy-MM-ddTHH-mm-ssZ")}.h5";

            return Directory.EnumerateFiles(folderPath, fileNamePattern).Any();
        }

        public override DataAvailabilityStatistics GetDataAvailabilityStatistics(string campaignName, DateTime begin, DateTime end)
        {
            this.EnsureOpened();

            ulong offset;
            int[] aggregatedData = null;
            DataAvailabilityGranularity granularity = DataAvailabilityGranularity.ChunkLevel;

            // epoch & hyperslab
            var epochStart = new DateTime(2000, 01, 01);
            var epochEnd = new DateTime(2030, 01, 01);

            if (!(epochStart <= begin && begin <= end && end <= epochEnd))
                throw new Exception("requirement >> epochStart <= dateTimeBegin && dateTimeBegin <= dateTimeEnd && dateTimeBegin <= epochEnd << is not matched");

            var samplesPerDay = new SampleRateContainer("is_chunk_completed_set").SamplesPerDay;

            var start = (ulong)Math.Floor((begin - epochStart).TotalDays * samplesPerDay);
            var block = (ulong)Math.Ceiling((end - begin).TotalDays * samplesPerDay);

            // get data
            var totalDays = (end - begin).TotalDays;

            this.SwitchLocation(() =>
            {
                var data = IOHelper.ReadDataset<byte>(_fileId, $"{campaignName}/is_chunk_completed_set", start, 1UL, block, 1UL).Select(value => (int)value).ToArray();

                if (totalDays <= 7)
                {
                    granularity = DataAvailabilityGranularity.ChunkLevel;
                    aggregatedData = data;
                }
                else if (totalDays <= 365)
                {
                    granularity = DataAvailabilityGranularity.DayLevel;
                    offset = (ulong)begin.TimeOfDay.TotalMinutes;
                    aggregatedData = new int[(int)Math.Ceiling(totalDays)];

                    Parallel.For(0, (int)Math.Ceiling(totalDays), day =>
                    {
                        var startIndex = (ulong)day * samplesPerDay; // inclusive
                        var endIndex = startIndex + samplesPerDay; // exclusive

                        if ((int)startIndex - (int)offset < 0)
                            startIndex = 0;
                        else
                            startIndex = startIndex - offset;

                        if (endIndex - offset >= (ulong)data.Length)
                            endIndex = (ulong)data.Length;
                        else
                            endIndex = endIndex - offset;

                        aggregatedData[day] = (int)((double)data.Skip((int)startIndex).Take((int)(endIndex - startIndex)).Sum() / (endIndex - startIndex) * 100);
                    });
                }
                else
                {
                    var totalMonths = (end.Month - begin.Month) + 1 + 12 * (end.Year - begin.Year);
                    var totalDateTimeBegin = new DateTime(begin.Year, begin.Month, 1);

                    granularity = DataAvailabilityGranularity.MonthLevel;
                    offset = (ulong)(begin - totalDateTimeBegin).TotalMinutes;
                    aggregatedData = new int[totalMonths];

                    Parallel.For(0, totalMonths, month =>
                    {
                        ulong startIndex; // inclusive
                        ulong endIndex; // exclusive

                        var currentDateTimeBegin = totalDateTimeBegin.AddMonths(month);
                        var currentDateTimeEnd = currentDateTimeBegin.AddMonths(1);

                        if ((currentDateTimeBegin - totalDateTimeBegin).TotalMinutes - offset < 0)
                            startIndex = 0;
                        else
                            startIndex = (ulong)(currentDateTimeBegin - totalDateTimeBegin).TotalMinutes - offset;

                        if ((currentDateTimeEnd - totalDateTimeBegin).TotalMinutes - offset >= data.Length)
                            endIndex = (ulong)data.Length;
                        else
                            endIndex = (ulong)(currentDateTimeEnd - totalDateTimeBegin).TotalMinutes - offset;

                        aggregatedData[month] = (int)((double)data.Skip((int)startIndex).Take((int)(endIndex - startIndex)).Sum() / (endIndex - startIndex) * 100);
                    });
                }
            });

            return new DataAvailabilityStatistics(granularity, aggregatedData);
        }

        public override void Dispose()
        {
            if (H5I.is_valid(_fileId) > 0) { H5F.close(_fileId); }
        }

        private void EnsureOpened()
        {
            if (!(H5I.is_valid(_fileId) > 0))
                _fileId = H5F.open(_filePath, H5F.ACC_RDONLY);
        }

        protected override (T[] dataset, byte[] statusSet) Read<T>(DatasetInfo dataset, ulong start, ulong length)
        {
            this.EnsureOpened();

            T[] data = null;
            byte[] statusSet = null;

            this.SwitchLocation(() =>
            {
                var datasetPath = dataset.GetPath();
                data = IOHelper.ReadDataset<T>(_fileId, datasetPath, start, 1, length, 1);

                if (H5L.exists(_fileId, datasetPath + "_status") > 0)
                    statusSet = IOHelper.ReadDataset(_fileId, datasetPath + "_status", start, 1, length, 1).Cast<byte>().ToArray();
            });

            return (data, statusSet);
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
