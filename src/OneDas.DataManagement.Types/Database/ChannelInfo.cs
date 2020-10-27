using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OneDas.DataManagement.Database
{
    [DebuggerDisplay("{Name,nq}")]
    public class ChannelInfo : ProjectElement
    {
        #region "Constructors"

        public ChannelInfo(string id, ProjectElement parent) : base(id, parent)
        {
            this.Name = string.Empty;
            this.Group = string.Empty;
            this.Unit = string.Empty;
            this.TransferFunctions = new List<TransferFunction>();
            this.Datasets = new List<DatasetInfo>();
        }

        private ChannelInfo()
        {
            //
        }

        #endregion

        #region "Properties"

        public string Name { get; set; }

        public string Group { get; set; }

        public string Unit { get; set; }

        public List<TransferFunction> TransferFunctions { get; set; }

        public List<DatasetInfo> Datasets { get; set; }

        #endregion

        #region "Methods"

        public void Merge(ChannelInfo channel, ChannelMergeMode mergeMode)
        {
            if (this.Parent.Id != channel.Parent.Id)
                throw new Exception("The channel to be merged has a different parent.");

            // merge properties
            switch (mergeMode)
            {
                case ChannelMergeMode.OverwriteMissing:
                    
                    if (string.IsNullOrWhiteSpace(this.Name))
                        this.Name = channel.Name;

                    if (string.IsNullOrWhiteSpace(this.Group))
                        this.Group = channel.Group;

                    if (string.IsNullOrWhiteSpace(this.Unit))
                        this.Unit = channel.Unit;

                    if (!this.TransferFunctions.Any())
                        this.TransferFunctions = channel.TransferFunctions;

                    break;

                case ChannelMergeMode.NewWins:
                    this.Name = channel.Name;
                    this.Group = channel.Group;
                    this.Unit = channel.Unit;
                    this.TransferFunctions = channel.TransferFunctions;
                    break;

                default:
                    throw new NotSupportedException();
            }

            // merge datasets
            var newDatasets = new List<DatasetInfo>();

            foreach (var dataset in channel.Datasets)
            {
                var referenceDataset = this.Datasets.FirstOrDefault(current => current.Id == dataset.Id);

                if (referenceDataset != null)
                    referenceDataset.Merge(dataset);
                else
                    newDatasets.Add(dataset);

                dataset.Parent = this;
            }

            this.Datasets.AddRange(newDatasets);
        }

        public override string GetPath()
        {
            return $"{this.Parent.GetPath()}/{this.Id}";
        }

        public override IEnumerable<ProjectElement> GetChilds()
        {
            return this.Datasets;
        }

        public override void Initialize()
        {
            base.Initialize();

            foreach (var dataset in this.Datasets)
            {
                dataset.Parent = this;
                dataset.Initialize();
            }
        }

        #endregion
    }
}
