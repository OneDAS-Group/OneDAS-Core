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
            if (this.Parent.Name != dataset.Parent.Name)
                throw new Exception("The dataset to be merged has a different parent.");

            if (this.DataType != dataset.DataType)
                throw new Exception("The dataset to be merged has a different data type.");
        }

        #endregion
    }
}