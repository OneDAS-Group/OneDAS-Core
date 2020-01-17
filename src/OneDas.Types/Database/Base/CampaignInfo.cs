using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace OneDas.Database
{
    [DebuggerDisplay("{Name,nq}")]
    [DataContract]
    public class CampaignInfo : InfoBase
    {
        #region "Constructors"

        public CampaignInfo(string name, InfoBase parent) : base(name, parent)
        {
            this.ChunkDatasetInfo = new DatasetInfo("is_chunk_completed_set", this);
            this.VariableInfos = new List<VariableInfo>();
        }

        #endregion

        #region "Properties"

        [DataMember]
        public DatasetInfo ChunkDatasetInfo { get; set; }

        [DataMember]
        public List<VariableInfo> VariableInfos { get; set; }

        #endregion

        #region "Methods"

        public override string GetPath()
        {
            return this.Name;
        }

        public override IEnumerable<InfoBase> GetChilds()
        {
            return this.VariableInfos;
        }

        #endregion
    }
}
