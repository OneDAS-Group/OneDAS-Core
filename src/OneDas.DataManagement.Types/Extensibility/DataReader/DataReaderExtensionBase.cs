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

        protected abstract (T[] dataset, byte[] statusSet) ReadPartial<T>(DatasetInfo dataset, TimeSpan offset, ulong length) where T : unmanaged;

        public void ReadFullDay<T>(DatasetInfo dataset, TimeSpan timeBase, Action<T[], byte[]> dataReceived) where T : unmanaged
        {
            while (true)
            {
                (T[] data, byte[] status) = this.ReadPartial<T>(dataset, offset, length);
                dataReceived?.Invoke(data, status);
            }
        }

        #endregion
    }
}
