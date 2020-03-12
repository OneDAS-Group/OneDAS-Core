using System;

namespace OneDas.DataManagement.Extensibility
{
    public class DataRecord
    {
        #region Constructors

        public DataRecord(Array dataset, byte[] status)
        {
            this.Dataset = dataset;
            this.Status = status;
        }

        #endregion

        #region Properties

        public Array Dataset { get; }

        public byte[] Status { get; }

        #endregion
    }
}
