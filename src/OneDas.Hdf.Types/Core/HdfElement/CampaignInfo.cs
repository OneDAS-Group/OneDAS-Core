using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OneDas.Hdf.Core
{
    [DataContract]
    public class CampaignInfo : HdfElementBase
    {
        public CampaignInfo(string name, DatasetInfo chunkDatasetInfo) : base(name)
        {
            this.ChunkDatasetInfo = chunkDatasetInfo;
            this.VariableInfoSet = new Dictionary<string, VariableInfo>();
        }

        [DataMember]
        public DatasetInfo ChunkDatasetInfo { get; set; }

        [DataMember]
        public Dictionary<string, VariableInfo> VariableInfoSet { get; set; }

        public override string GetPath()
        {
            return this.Name;
        }

        public override string GetDisplayName()
        {
            return this.Name;
        }

        public override IEnumerable<HdfElementBase> GetChildSet()
        {
            return VariableInfoSet.Values;
        }
    }
}
