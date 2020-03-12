using System;

namespace OneDas.DataManagement.Extensibility
{
    public class DataRecord
    {
        #region Constructors

        public DataRecord(Array dataset, byte[] statusSet)
        {
            this.Dataset = dataset;
            this.StatusSet = statusSet;
        }

        #endregion

        #region Properties

        public Array Dataset { get; }

        public byte[] StatusSet { get; }

        #endregion
    }
}
