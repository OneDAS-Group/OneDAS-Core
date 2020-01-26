using OneDas.Database;
using System;
using System.Collections.Generic;

namespace OneDas.DataManagement.Extensibility
{
    public abstract class DataReaderExtensionBase : IDisposable
    {
        #region Properties

        public abstract bool IsDataAvailable { get; }

        #endregion

        #region Methods

        public abstract CampaignInfo GetCampaignInfo();

        public abstract (int first, int last) GetCompletedChunkBounds();

        public abstract List<int> GetVersions();

        public abstract void Dispose();

        protected abstract (T[] dataset, byte[] statusSet) ReadPartial<T>(DatasetInfo dataset, ulong start, ulong length) where T : unmanaged;

        public void ReadFullDay<T>(DatasetInfo dataset, TimeSpan fundamentalPeriod, ulong samplesPerFundamentalPeriod, ulong maxSamplesPerReadOperation, Action<T[], byte[]> dataReceived) where T : unmanaged
        {
            // check that the 'fundamentalPeriod' is an integer fraction of the maximum period.
            var maxPeriod = TimeSpan.FromDays(1);

            if (maxPeriod < fundamentalPeriod)
                throw new Exception("The fundamental period must be less than or equal to the maximum period.");

            var tmp = maxPeriod.Ticks / (double)fundamentalPeriod.Ticks;

            if (tmp % 1 != 0)
                throw new Exception("The provided fundamental period is not an integer fraction of the maximum period.");

            var totalFpCount = (long)tmp;

            // max fundamental periods count
            var maxFpCount = maxSamplesPerReadOperation / samplesPerFundamentalPeriod;

            if (maxFpCount == 0)
                throw new Exception("The provided 'maximum samples per read operation' parameter is too small to provide data for at least a single fundamental period.");

            // load data
            var timeOffset = TimeSpan.Zero;

            while (timeOffset < maxPeriod)
            {
                var remainingFpCount = (maxPeriod - timeOffset).Ticks / fundamentalPeriod.Ticks;
                var currentFpCount = Math.Min(remainingFpCount, maxFpCount);
                var length = currentFpCount * samplesPerFp;
                var offset = (totalFpCount - remainingFpCount) * samplesPerFp;

                (T[] data, byte[] status) = this.ReadPartial<T>(dataset, offset, length);
                dataReceived?.Invoke(data, status);

                timeOffset += fundamentalPeriod * currentFpCount;
            }
        }

        #endregion
    }
}
