using System.Collections.Generic;

namespace OneDas.DataManagement.Database
{
    public class OneDasDatabase
    {
        #region Constructors

        public OneDasDatabase()
        {
            this.RootPathToDataReaderIdMap = new Dictionary<string, string>();
            this.CampaignContainers = new List<CampaignContainer>();
        }

        #endregion

        #region Properties

        public Dictionary<string, string> RootPathToDataReaderIdMap { get; set; }
        public List<CampaignContainer> CampaignContainers { get; set; }

        #endregion

        #region Methods

        public void Initialize()
        {
            foreach (var container in this.CampaignContainers)
            {
                container.Initialize();
            }
        }

        #endregion
    }
}