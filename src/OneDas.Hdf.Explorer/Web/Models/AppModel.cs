using OneDas.Database;
using OneDas.Hdf.Explorer.Core;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OneDas.Hdf.Explorer.Web
{
    [DataContract]
    public class AppModel
    {
        public AppModel(HdfExplorerState hdfExplorerState, List<CampaignInfo> campaignInfoSet)
        {
            this.HdfExplorerState = hdfExplorerState;
            this.CampaignInfoSet = campaignInfoSet;
        }

        [DataMember]
        public HdfExplorerState HdfExplorerState;

        [DataMember]
        public readonly List<CampaignInfo> CampaignInfoSet;
    }
}
