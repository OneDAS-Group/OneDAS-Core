using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OneDas.DataManagement.Database
{
    [DebuggerDisplay("{Id,nq}")]
    public class DatasetInfo : CampaignElement
    {
        #region "Constructors"

        public DatasetInfo(string id, CampaignElement parent) : base(id, parent)
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

#warning Encode SamplesPerDay in DatasetInfo instead of name?

        public SampleRateContainer SampleRate => new SampleRateContainer(this.Id);

        #endregion

        #region "Methods"

        public override string GetPath()
        {
            return $"{this.Parent.GetPath()}/{this.Id}";
        }

        public override IEnumerable<CampaignElement> GetChilds()
        {
            return new List<DatasetInfo> { };
        }

        public void Merge(DatasetInfo dataset)
        {
            if (this.Parent.Id != dataset.Parent.Id
                || this.DataType != dataset.DataType
                || this.IsNative != dataset.IsNative)
                throw new Exception("The datasets to be merged are not equal.");
        }

        #endregion
    }
}