using System.Collections.Generic;

namespace OneDas.DataManagement.Database
{
    public class OneDasDatabaseConfig
    {
        #region Constructors

        public OneDasDatabaseConfig()
        {
            this.AggregationDataReaderRootPath = "";
            this.RootPathToDataReaderIdMap = new Dictionary<string, string>();
        }

        #endregion

        #region Properties

        public string AggregationDataReaderRootPath { get; set; }

        public Dictionary<string, string> RootPathToDataReaderIdMap { get; set; }

        #endregion

        #region Methods

        public void Initialize()
        {
            this.RootPathToDataReaderIdMap[":memory:"] = "OneDas.InMemory";
        }

        #endregion
    }
}