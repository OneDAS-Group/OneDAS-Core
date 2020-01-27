using System.Collections.Generic;
using System.Linq;

namespace OneDas.Data
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

        public List<CampaignInfo> GetCampaigns()
        {
            return this.CampaignContainers.Select(container => container.Campaign).ToList();
        }

        #endregion
    }
}