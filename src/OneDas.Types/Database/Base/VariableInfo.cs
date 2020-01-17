using OneDas.Infrastructure;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace OneDas.Database
{
    [DebuggerDisplay("{GetDisplayName(),nq}")]
    [DataContract]
    public class VariableInfo : InfoBase
    {
        #region "Constructors"

        public VariableInfo(string name, InfoBase parent) : base(name, parent)
        {
            this.VariableNames = new List<string>();
            this.VariableGroups = new List<string>();
            this.Units = new List<string>();
            this.TransferFunctions = new List<TransferFunction>();

            this.DatasetInfos = new List<DatasetInfo>();
        }

        #endregion

        #region "Properties"

        [DataMember]
        public List<string> VariableNames { get; set; }

        [DataMember]
        public List<string> VariableGroups { get; set; }

        [DataMember]
        public List<string> Units { get; set; }

        [DataMember]
        public List<TransferFunction> TransferFunctions { get; set; }

        [DataMember]
        public List<DatasetInfo> DatasetInfos { get; }

        #endregion

        #region "Methods"

        public string GetDisplayName()
        {
            return this.VariableNames.Last();
        }

        public override string GetPath()
        {
            return $"{this.Parent.GetPath()}/{this.Name}";
        }

        public override IEnumerable<InfoBase> GetChilds()
        {
            return this.DatasetInfos;
        }

        #endregion
    }
}
