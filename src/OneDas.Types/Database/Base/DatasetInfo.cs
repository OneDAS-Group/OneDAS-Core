using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OneDas.Database
{
    [DataContract]
    public class DatasetInfo : InfoBase
    {
        #region "Constructors"

        public DatasetInfo(string name, InfoBase parent) : base(name, parent)
        {
            //
        }

        #endregion

        #region "Methods"

        public override string GetPath()
        {
            return $"{ this.Parent.GetPath() }/{ this.Name }";
        }

        public override IEnumerable<InfoBase> GetChilds()
        {
            return new List<DatasetInfo> { };
        }

        #endregion
    }
}