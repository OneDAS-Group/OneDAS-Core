using OneDas.Database;
using OneDas.DataStorage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OneDas.DataManagement.Extensibility
{
    public abstract class DataReaderExtensionBase : IDisposable
    {
        #region Methods

        public void ReadFullDay<T>(DatasetInfo dataset, TimeSpan fundamentalPeriod, ulong samplesPerFundamentalPeriod, ulong maxSamplesPerReadOperation, Action<T[], byte[]> processData) where T : unmanaged
        {
            // max period
            var maxPeriod = TimeSpan.FromDays(1);

            if (maxPeriod < fundamentalPeriod)
                throw new Exception("The fundamental period must be less than or equal to the maximum period.");

            var tmp = maxPeriod.Ticks / (double)fundamentalPeriod.Ticks;

            if (tmp % 1 != 0)
                throw new Exception("The provided fundamental period is not an integer fraction of the maximum period.");

            var totalFpCount = (ulong)tmp;

            // max fundamental periods count
            var maxFpCount = maxSamplesPerReadOperation / samplesPerFundamentalPeriod;

            if (maxFpCount == 0)
                throw new Exception("The provided 'maximum samples per read operation' parameter is too small to provide data for at least a single fundamental period.");

            // load data
            var timeOffset = TimeSpan.Zero;

            while (timeOffset < maxPeriod)
            {
                var remainingFpCount = (ulong)((maxPeriod - timeOffset).Ticks / fundamentalPeriod.Ticks);
                var currentFpCount = Math.Min(remainingFpCount, maxFpCount);
                var length = currentFpCount * samplesPerFundamentalPeriod;
                var offset = (totalFpCount - remainingFpCount) * samplesPerFundamentalPeriod;

                (T[] data, byte[] status) = this.Read<T>(dataset, offset, length);
                processData?.Invoke(data, status);

                timeOffset += fundamentalPeriod * currentFpCount;
            }
        }

        public ISimpleDataStorage LoadDataset<T>(DatasetInfo dataset, ulong start, ulong length) where T : unmanaged
        {
            this.Open();

            (var data, var statusSet) = this.Read<T>(dataset, start, length);

            // apply status (only if native dataset)
            if (statusSet != null)
            {
                using var extendedDataStorage = new ExtendedDataStorage<T>(data, statusSet);
                var simpleDataStorage = extendedDataStorage.ToSimpleDataStorage();

                return simpleDataStorage;
            }
            else
            {
                return new SimpleDataStorage(data.Cast<double>().ToArray());
            }
        }

        public abstract List<CampaignInfo> GetCampaigns();

        public abstract bool IsDataOfDayAvailable(DateTime dateTime);


        public abstract DataAvailabilityStatistics GetDataAvailabilityStatistics(string campaignName, DateTime dateTimeBegin, DateTime dateTimeEnd);

        public abstract void Open();

        public abstract void Close();

        public abstract void Dispose();

        protected abstract (T[] dataset, byte[] statusSet) Read<T>(DatasetInfo dataset, ulong start, ulong length) where T : unmanaged;

        #endregion
    }
}
