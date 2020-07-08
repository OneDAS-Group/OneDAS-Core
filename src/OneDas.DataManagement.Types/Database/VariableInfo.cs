using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OneDas.DataManagement.Database
{
    [DebuggerDisplay("{Name,nq}")]
    public class VariableInfo : CampaignElement
    {
        #region "Constructors"

        public VariableInfo(string id, CampaignElement parent) : base(id, parent)
        {
            this.Name = string.Empty;
            this.Group = string.Empty;
            this.Unit = string.Empty;
            this.TransferFunctions = new List<TransferFunction>();
            this.Datasets = new List<DatasetInfo>();
        }

        private VariableInfo()
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

        public void Merge(VariableInfo variable, VariableMergeMode mergeMode)
        {
            if (this.Parent.Id != variable.Parent.Id)
                throw new Exception("The variable to be merged has a different parent.");

            // merge properties
            switch (mergeMode)
            {
                case VariableMergeMode.OverwriteMissing:
                    
                    if (string.IsNullOrWhiteSpace(this.Name))
                        this.Name = variable.Name;

                    if (string.IsNullOrWhiteSpace(this.Group))
                        this.Group = variable.Group;

                    if (string.IsNullOrWhiteSpace(this.Unit))
                        this.Unit = variable.Unit;

                    if (!this.TransferFunctions.Any())
                        this.TransferFunctions = variable.TransferFunctions;

                    break;

                case VariableMergeMode.NewWins:
                    this.Name = variable.Name;
                    this.Group = variable.Group;
                    this.Unit = variable.Unit;
                    this.TransferFunctions = variable.TransferFunctions;
                    break;

                default:
                    throw new NotSupportedException();
            }

            // merge datasets
            var newDatasets = new List<DatasetInfo>();

            foreach (var dataset in variable.Datasets)
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

        public override IEnumerable<CampaignElement> GetChilds()
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
