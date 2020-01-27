using OneDas.Data;
using OneDas.Hdf.Explorer.Core;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OneDas.Hdf.Explorer.Web
{
    [DataContract]
    public class AppModel
    {
        public AppModel(OneDasExplorerState hdfExplorerState, List<CampaignInfo> campaignInfoSet)
        {
            this.HdfExplorerState = hdfExplorerState;
            this.CampaignInfoSet = campaignInfoSet;
        }

        [DataMember]
        public OneDasExplorerState HdfExplorerState;

        [DataMember]
        public readonly List<CampaignInfo> CampaignInfoSet;
    }
}
