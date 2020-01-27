using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace OneDas.Data
{
    [DebuggerDisplay("{Name,nq}")]
    [DataContract]
    public class DatasetInfo : InfoBase
    {
        #region "Constructors"

        public DatasetInfo(string name, InfoBase parent) : base(name, parent)
        {
            //
        }

        #endregion

        #region Properties

        public OneDasDataType DataType { get; set; }

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

        public void Merge(DatasetInfo dataset)
        {
            if (this.Parent.Name != dataset.Parent.Name)
                throw new Exception("The dataset to be merged has a different parent.");

            if (this.DataType != dataset.DataType)
                throw new Exception("The dataset to be merged has a different data type.");
        }

        #endregion
    }
}