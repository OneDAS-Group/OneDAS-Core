using System.Collections.Generic;

namespace OneDas.DataManagement.Database
{
    public class OneDasDatabaseConfig
    {
        #region Constructors

        public OneDasDatabaseConfig()
        {
            this.RootPathToDataReaderIdMap = new Dictionary<string, string>();
            this.AggregationConfigs = new List<AggregationConfig>();
            this.RestrictedCampaigns = new List<string>();
        }

        #endregion

        #region Properties

        public Dictionary<string, string> RootPathToDataReaderIdMap { get; set; }

        public List<AggregationConfig> AggregationConfigs { get; set; }

        public List<string> RestrictedCampaigns { get; set; }

        #endregion
    }
}