using OneDas.DataManagement.Database;
using OneDas.DataManagement.Explorer.Core;
using System.Collections.Generic;

namespace OneDas.DataManagement.Explorer.Web
{
    public class AppModel
    {
        public AppModel(OneDasExplorerState explorerState, List<CampaignInfo> campaignInfoSet)
        {
            this.ExplorerState = explorerState;
            this.Campaigns = campaignInfoSet;
        }

        private AppModel()
        {
            //
        }

        public OneDasExplorerState ExplorerState { get; set; }

        public List<CampaignInfo> Campaigns { get; set; }
    }
}
