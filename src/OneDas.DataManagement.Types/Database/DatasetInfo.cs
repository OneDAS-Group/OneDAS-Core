using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OneDas.DataManagement.Database
{
    [DebuggerDisplay("{Name,nq}")]
    public class DatasetInfo : InfoBase
    {
        #region "Constructors"

        public DatasetInfo(string name, InfoBase parent) : base(name, parent)
        {
            //
        }

        private DatasetInfo()
        {
            //
        }

        #endregion

        #region Properties

        public OneDasDataType DataType { get; set; }

        public bool IsNative { get; set; } = true;

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
            if (this.Parent.Name != dataset.Parent.Name
                || this.DataType != dataset.DataType
                || this.IsNative != dataset.IsNative)
                throw new Exception("The datasets to be merged are not equal.");
        }

        #endregion
    }
}