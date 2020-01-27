using System;
using System.Diagnostics;

namespace OneDas.Data
{
    [DebuggerDisplay("{Name,nq}")]
    public class CampaignContainer
    {
        #region "Constructors"

        public CampaignContainer(string campaignName, string rootPath)
        {
            this.Name = campaignName;
            this.RootPath = rootPath;
            this.Campaign = new CampaignInfo(campaignName);
            this.CampaignMeta = new CampaignMetaInfo();
        }

        private CampaignContainer()
        {
            //
        }

        #endregion

        #region "Properties"

        public string Name { get; set; }

        public string RootPath { get; set; }

        public DateTime LastScan { get; set; }

        public CampaignInfo Campaign { get; set; }

        public CampaignMetaInfo CampaignMeta { get; set; }

        #endregion
    }
}