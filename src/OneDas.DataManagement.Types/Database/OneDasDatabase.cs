using System.Collections.Generic;

namespace OneDas.DataManagement.Database
{
    public class OneDasDatabase
    {
        #region Constructors

        public OneDasDatabase()
        {
            this.CampaignContainers = new List<CampaignContainer>();
        }

        #endregion

        #region Properties

        public List<CampaignContainer> CampaignContainers { get; set; }

        #endregion
    }
}