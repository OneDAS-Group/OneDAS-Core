using System.Collections.Generic;
using System.Runtime.Serialization;
using OneDas.Hdf.Core;
using OneDas.Hdf.Explorer.Core;

namespace OneDas.Hdf.Explorer.Web
{
    [DataContract]
    public class AppModel
    {
        public AppModel(HdfExplorerState hdfExplorerState, List<CampaignInfo> campaignInfoSet, Dictionary<string, string> campaignDescriptionSet)
        {
            this.HdfExplorerState = hdfExplorerState;
            this.CampaignInfoSet = campaignInfoSet;
            this.CampaignDescriptionSet = campaignDescriptionSet;
        }

        [DataMember]
        public HdfExplorerState HdfExplorerState;

        [DataMember]
        public readonly List<CampaignInfo> CampaignInfoSet;

        [DataMember]
        public readonly Dictionary<string, string> CampaignDescriptionSet;
    }
}
