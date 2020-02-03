using System.Collections.Generic;

namespace OneDas.DataManagement.Database
{
    public class OneDasDatabaseConfig
    {
        #region Constructors

        public OneDasDatabaseConfig()
        {
            this.RootPathToDataReaderIdMap = new Dictionary<string, string>();
        }

        #endregion

        #region Properties

        public Dictionary<string, string> RootPathToDataReaderIdMap { get; set; }

        #endregion
    }
}