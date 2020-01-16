using OneDas.Infrastructure;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OneDas.Database
{
    [DataContract]
    public class VariableInfo : InfoBase
    {
        #region "Constructors"

        public VariableInfo(string name, InfoBase parent) : base(name, parent)
        {
            this.VariableNameSet = new List<string>();
            this.VariableGroupSet = new List<string>();
            this.UnitSet = new List<string>();
            this.TransferFunctionSet = new List<TransferFunction>();

            this.DatasetInfoSet = new List<DatasetInfo>();
        }

        #endregion

        #region "Properties"

        [DataMember]
        public List<string> VariableNameSet { get; set; }

        [DataMember]
        public List<string> VariableGroupSet { get; set; }

        [DataMember]
        public List<string> UnitSet { get; set; }

        [DataMember]
        public List<TransferFunction> TransferFunctionSet { get; set; }

        [DataMember]
        public List<DatasetInfo> DatasetInfoSet { get; }

        #endregion

        #region "Methods"

        public override string GetPath()
        {
            return $"{this.Parent.GetPath()}/{this.Name}";
        }

        public override IEnumerable<InfoBase> GetChildSet()
        {
            return this.DatasetInfoSet;
        }

        #endregion
    }
}
