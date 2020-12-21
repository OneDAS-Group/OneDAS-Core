using System.Collections.Generic;

namespace OneDas.DataManagement.Database
{
    public class OneDasDatabaseConfig
    {
        #region Constructors

        public OneDasDatabaseConfig()
        {
            this.AggregationDataReaderRootPath = "";
            this.DataReaderRegistrations = new List<DataReaderRegistration>();
        }

        #endregion

        #region Properties

        public string AggregationDataReaderRootPath { get; set; }

        public List<DataReaderRegistration> DataReaderRegistrations { get; set; }

        #endregion
    }
}