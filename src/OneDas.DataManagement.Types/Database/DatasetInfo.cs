using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OneDas.DataManagement.Database
{
    [DebuggerDisplay("{Name,nq}")]
    public class Dataset : InfoBase
    {
        #region "Constructors"

        public Dataset(string name, InfoBase parent) : base(name, parent)
        {
            //
        }

        private Dataset()
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
            return new List<Dataset> { };
        }

        public void Merge(Dataset dataset)
        {
            if (this.Parent.Name != dataset.Parent.Name
                || this.DataType != dataset.DataType
                || this.IsNative != dataset.IsNative)
                throw new Exception("The datasets to be merged are not equal.");
        }

        #endregion
    }
}