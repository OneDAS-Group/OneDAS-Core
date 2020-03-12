using OneDas.DataManagement.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace OneDas.DataManagement.Extensibility
{
    public abstract class DataReaderExtensionBase : IDisposable
    {
        #region Constructors

        public DataReaderExtensionBase(string rootPath)
        {
            this.RootPath = rootPath;
            this.Progress = new Progress<double>();
        }

        #endregion

        #region Properties

        public string RootPath { get; }

        public Progress<double> Progress { get; }

        #endregion

        #region Methods

        public void Read(DatasetInfo dataset,
                         DateTime begin,
                         DateTime end,
                         ulong upperBlockSize,
                         Action<DataReaderProgressRecord> dataAvailable,
                         CancellationToken cancellationToken)
        {
            this.Read(new List<DatasetInfo>() { dataset }, begin, end, upperBlockSize, TimeSpan.FromMinutes(1), dataAvailable, cancellationToken);
        }

        public void Read(List<DatasetInfo> datasets,
                         DateTime begin,
                         DateTime end,
                         ulong upperBlockSize,
                         Action<DataReaderProgressRecord> dataAvailable,
                         CancellationToken cancellationToken)
        {
            this.Read(datasets, begin, end, upperBlockSize, TimeSpan.FromMinutes(1), dataAvailable, cancellationToken);
        }

        public void Read(DatasetInfo dataset,
                         DateTime begin,
                         DateTime end,
                         ulong upperBlockSize,
                         TimeSpan fundamentalPeriod,
                         Action<DataReaderProgressRecord> dataAvailable,
                         CancellationToken cancellationToken)
        {
            this.Read(new List<DatasetInfo>() { dataset }, begin, end, upperBlockSize, fundamentalPeriod, dataAvailable, cancellationToken);
        }

        public void Read(List<DatasetInfo> datasets,
                         DateTime begin,
                         DateTime end,
                         ulong blockSizeLimit,
                         TimeSpan fundamentalPeriod,
                         Action<DataReaderProgressRecord> dataAvailable,
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
                return;

            if (!datasets.Any() || begin == end)
                return;

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
                var samplesPerMinute = dataset.SampleRate.SamplesPerSecond * 60;
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
                        return;

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
                dataAvailable?.Invoke(new DataReaderProgressRecord(datasetToRecordMap, currentBegin, currentEnd));

                // continue in time
                currentBegin += currentPeriod;
                remainingPeriod = end - currentBegin;
            }
        }

        public abstract List<string> GetCampaignNames();

        public abstract CampaignInfo GetCampaign(string campaignName);

        public abstract bool IsDataOfDayAvailable(string campaignName, DateTime date);

        public abstract DataAvailabilityStatistics GetDataAvailabilityStatistics(string campaignName, DateTime begin, DateTime end);

        public abstract void Dispose();

        protected abstract (T[] dataset, byte[] statusSet) ReadSingle<T>(DatasetInfo dataset, DateTime begin, DateTime end) where T : unmanaged;

        private (Array dataset, byte[] statusSet) InternalReadSingle<T>(DatasetInfo dataset, DateTime begin, DateTime end) where T : unmanaged
        {
            (T[] data, byte[] statusSet) = this.ReadSingle<T>(dataset, begin, end);
            return (data, statusSet);
        }

        #endregion
    }
}
