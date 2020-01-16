using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OneDas.Database
{
    [DataContract]
    public class CampaignInfo : InfoBase
    {
        #region "Constructors"

        public CampaignInfo(string name, InfoBase parent) : base(name, parent)
        {
            ChunkDatasetInfo = new DatasetInfo("is_chunk_completed_set", this);
            VariableInfoSet = new List<VariableInfo>();
        }

        #endregion

        #region "Properties"

        [DataMember]
        public DatasetInfo ChunkDatasetInfo { get; }

        [DataMember]
        public List<VariableInfo> VariableInfoSet { get; }

        #endregion

        #region "Methods"

        public override string GetPath()
        {
            return this.Name;
        }

        public override IEnumerable<InfoBase> GetChildSet()
        {
            return this.VariableInfoSet;
        }

        #endregion
    }
}
