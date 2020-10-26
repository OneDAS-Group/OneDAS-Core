using Microsoft.Extensions.Logging;
using OneDas.DataManagement.Database;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Extensibility
{
    public abstract class DataReaderExtensionBase : IDisposable
    {
        #region Constructors

        public DataReaderExtensionBase(string rootPath, ILogger logger)
        {
            this.RootPath = rootPath;
            this.Logger = logger;
            this.Progress = new Progress<double>();
        }

        #endregion

        #region Properties

        public string RootPath { get; }

        public ILogger Logger { get; }

        public Progress<double> Progress { get; }

        public List<CampaignInfo> Campaigns { get; private set; }

        #endregion

        #region Methods

        public void InitializeCampaigns()
        {
            this.Campaigns = this.LoadCampaigns();
        }

        public void InitializeCampaigns(List<CampaignInfo> campaigns)
        {
            this.Campaigns = campaigns;
        }

        public DataReaderStream ReadAsStream(
            DatasetInfo dataset,
            DateTime begin,
            DateTime end,
            ulong upperBlockSize)
        {
            var cts = new CancellationTokenSource();
            var progressRecords = this.Read(new List<DatasetInfo>() { dataset }, begin, end, upperBlockSize, TimeSpan.FromMinutes(1), cts.Token);
            var samplesPerSecond = new SampleRateContainer(dataset.Id).SamplesPerSecond;
            var length = (long)Math.Round(samplesPerSecond * (decimal)(end - begin).TotalSeconds, MidpointRounding.AwayFromZero);

            return new DataReaderStream(length, progressRecords, cts);
        }

        public IEnumerable<DataReaderProgressRecord> Read(
            DatasetInfo dataset,
            DateTime begin,
            DateTime end,
            ulong upperBlockSize,
            CancellationToken cancellationToken)
        {
            return this.Read(new List<DatasetInfo>() { dataset }, begin, end, upperBlockSize, TimeSpan.FromMinutes(1), cancellationToken);
        }

        public IEnumerable<DataReaderProgressRecord> Read(
            List<DatasetInfo> datasets,
            DateTime begin,
            DateTime end,
            ulong upperBlockSize,
            CancellationToken cancellationToken)
        {
            return this.Read(datasets, begin, end, upperBlockSize, TimeSpan.FromMinutes(1), cancellationToken);
        }

        public IEnumerable<DataReaderProgressRecord> Read(
            DatasetInfo dataset,
            DateTime begin,
            DateTime end,
            ulong upperBlockSize,
            TimeSpan fundamentalPeriod,
            CancellationToken cancellationToken)
        {
            return this.Read(new List<DatasetInfo>() { dataset }, begin, end, upperBlockSize, fundamentalPeriod, cancellationToken);
        }

        public IEnumerable<DataReaderProgressRecord> Read(
            List<DatasetInfo> datasets,
            DateTime begin,
            DateTime end,
            ulong blockSizeLimit,
            TimeSpan fundamentalPeriod,
            CancellationToken cancellationToken)
        {
            /* 
             * |....................|
             * |
             * |
             * |....................
             * |
             * |
             * |....................
             * |
             * |====================
             * |....................
             * |
             * |
             * |....................|
             * 
             * |     = base period (1 minute)
             *  ...  = fundamental period (e.g. 10 minutes)
             * |...| = begin & end markers
             *  ===  = block period
             */

            if (cancellationToken.IsCancellationRequested)
                yield break;

            if (!datasets.Any() || begin == end)
                yield break;

            var minute = TimeSpan.FromMinutes(1);
            var period = end - begin;

            // sanity checks
            if (begin.Ticks % minute.Ticks != 0)
                throw new Exception("The begin parameter must be a multiple of 1 minute.");

            if (end.Ticks % minute.Ticks != 0)
                throw new Exception("The end parameter must be a multiple of 1 minute.");

            if (fundamentalPeriod.Ticks % minute.Ticks != 0)
                throw new Exception("The fundamental period parameter must be a multiple of 1 minute.");

            if (period.Ticks % fundamentalPeriod.Ticks != 0)
                throw new Exception("The request period must be a multiple of the fundamental period.");

            if (blockSizeLimit == 0)
                throw new Exception("The upper block size must be > 0 bytes.");

            // calculation
            var minutesPerFP = fundamentalPeriod.Ticks / minute.Ticks;

            var bytesPerFP = datasets.Sum(dataset =>
            {
                var bytesPerSample = OneDasUtilities.SizeOf(dataset.DataType);
                var samplesPerMinute = dataset.GetSampleRate().SamplesPerSecond * 60;
                var bytesPerFP = bytesPerSample * samplesPerMinute * minutesPerFP;

                return bytesPerFP;
            });

            var FPCountPerBlock = blockSizeLimit / bytesPerFP;
            var roundedFPCount = (long)Math.Floor(FPCountPerBlock);

            if (roundedFPCount < 1)
                throw new Exception("The block size limit is too small.");

            var maxPeriodPerRequest = TimeSpan.FromTicks(fundamentalPeriod.Ticks * roundedFPCount);

            // load data
            var currentBegin = begin;
            var remainingPeriod = end - currentBegin;

            while (remainingPeriod > TimeSpan.Zero)
            {
                var datasetToRecordMap = new Dictionary<DatasetInfo, DataRecord>();
                var currentPeriod = TimeSpan.FromTicks(Math.Min(remainingPeriod.Ticks, maxPeriodPerRequest.Ticks));
                var currentEnd = currentBegin + currentPeriod;
                var index = 1;
                var count = datasets.Count;

                foreach (var dataset in datasets)
                {
                    if (cancellationToken.IsCancellationRequested)
                        yield break;

                    // invoke generic method
                    var type = typeof(DataReaderExtensionBase);
                    var flags = BindingFlags.Instance | BindingFlags.NonPublic;
                    var genericType = OneDasUtilities.GetTypeFromOneDasDataType(dataset.DataType);
                    var parameters = new object[] { dataset, currentBegin, currentEnd };
                    var result = OneDasUtilities.InvokeGenericMethod(type, this, nameof(this.InternalReadSingle), flags, genericType, parameters);
                    (var data, var statusSet) = ((Array data, byte[] statusSet))result;

                    datasetToRecordMap[dataset] = new DataRecord(data, statusSet);

                    // update progress
                    var localProgress = TimeSpan.FromTicks(currentPeriod.Ticks * index / count);
                    var currentProgress = (currentBegin + localProgress - begin).Ticks / (double)period.Ticks;

                    ((IProgress<double>)this.Progress).Report(currentProgress);
                    index++;
                }

                // notify about new data
                yield return new DataReaderProgressRecord(datasetToRecordMap, currentBegin, currentEnd);

                // continue in time
                currentBegin += currentPeriod;
                remainingPeriod = end - currentBegin;
            }
        }

        public DataAvailabilityStatistics GetDataAvailabilityStatistics(string campaignId, DateTime begin, DateTime end)
        {
            var dateBegin = begin.Date;
            var dateEnd = end.Date;

            int[] aggregatedData = default;
            var granularity = DataAvailabilityGranularity.DayLevel;
            var totalDays = (int)(dateEnd - dateBegin).TotalDays;

            if (totalDays <= 365)
            {
                aggregatedData = new int[totalDays];

                Parallel.For(0, totalDays, day =>
                {
                    var date = dateBegin.AddDays(day);
                    aggregatedData[day] = (int)(this.GetDataAvailability(campaignId, date) * 100);
                });
            }
            else
            {
                granularity = DataAvailabilityGranularity.MonthLevel;

                var months = new DateTime[totalDays];
                var datasets = new int[totalDays];

                Parallel.For(0, totalDays, day =>
                {
                    var date = dateBegin.AddDays(day);
                    var month = new DateTime(date.Year, date.Month, 1);

                    months[day] = month;
                    datasets[day] = (int)(this.GetDataAvailability(campaignId, date) * 100);
                });

                var uniqueMonths = months.Distinct().OrderBy(month => month).ToList();
                var zipData = months.Zip(datasets, (month, dataset) => (month, dataset)).ToList();

                aggregatedData = new int[uniqueMonths.Count];

                for (int i = 0; i < uniqueMonths.Count; i++)
                {
                    aggregatedData[i] = (int)zipData
                        .Where(current => current.month == uniqueMonths[i])
                        .Average(current => current.dataset);
                }
            }

            return new DataAvailabilityStatistics(granularity, aggregatedData);
        }


        public List<string> GetCampaignNames()
        {
            return this.Campaigns.Select(campaign => campaign.Id).ToList();
        }

        public CampaignInfo GetCampaign(string campaignId)
        {
            return this.Campaigns.First(campaign => campaign.Id == campaignId);
        }

        public bool IsDataOfDayAvailable(string campaignId, DateTime day)
        {
            return this.GetDataAvailability(campaignId, day) > 0;
        }

        public abstract (T[] Dataset, byte[] StatusSet) ReadSingle<T>(DatasetInfo dataset, DateTime begin, DateTime end) where T : unmanaged;

        public virtual void Dispose()
        {
            //
        }

        protected abstract List<CampaignInfo> LoadCampaigns();

        protected abstract double GetDataAvailability(string campaignId, DateTime Day);

        private (Array dataset, byte[] statusSet) InternalReadSingle<T>(DatasetInfo dataset, DateTime begin, DateTime end) where T : unmanaged
        {
            (T[] data, byte[] statusSet) = this.ReadSingle<T>(dataset, begin, end);
            return (data, statusSet);
        }

        #endregion
    }
}
