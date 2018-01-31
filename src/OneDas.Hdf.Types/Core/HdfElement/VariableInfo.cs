using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using OneDas.Hdf.IO;

namespace OneDas.Hdf.Core
{
    [DataContract]
    public class VariableInfo : HdfElementBase
    {
        #region "Constructors"

        public VariableInfo(string name) : base(name)
        {
            this.VariableNameSet = new List<string>();
            this.VariableGroupSet = new List<string>();
            this.UnitSet = new List<string>();
            this.TransferFunctionSet = new List<hdf_transfer_function_t>();
            this.DatasetInfoSet = new Dictionary<string, DatasetInfo>();
        }

        #endregion

        #region "Methods"

        [DataMember]
        public IList<string> VariableNameSet { get; set; }

        [DataMember]
        public IList<string> VariableGroupSet { get; set; }

        [DataMember]
        public IList<string> UnitSet { get; set; }

        [DataMember]
        public IList<hdf_transfer_function_t> TransferFunctionSet { get; set; }

        [DataMember]
        public Dictionary<string, DatasetInfo> DatasetInfoSet { get; set; }

        #endregion

        #region "Methods"

        public override string GetPath()
        {
            return $"{this.Parent.GetPath()}/{this.Name}";
        }

        public override string GetDisplayName()
        {
            return $"{new string(VariableNameSet.Last().Take(55).ToArray()),-55}\t({new string(this.Name.Take(8).ToArray())}...)";
        }

        public override IEnumerable<HdfElementBase> GetChildSet()
        {
            return DatasetInfoSet.Values;
        }

        #endregion
    }
}
