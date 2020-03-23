using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OneDas.DataManagement.Database
{
    [DebuggerDisplay("{GetDisplayName(),nq}")]
    public class VariableInfo : CampaignElement
    {
#warning Ensure the properties are never zero (right now they may be set from outside)
        #region "Constructors"

        public VariableInfo(string id, CampaignElement parent) : base(id, parent)
        {
            this.VariableNames = new List<string>();
            this.VariableGroups = new List<string>();
            this.Units = new List<string>();
            this.TransferFunctions = new List<TransferFunction>();

            this.Datasets = new List<DatasetInfo>();
        }

        private VariableInfo()
        {
            //
        }

        #endregion

        #region "Properties"

        public List<string> VariableNames { get; set; }

        public List<string> VariableGroups { get; set; }

        public List<string> Units { get; set; }

        public List<TransferFunction> TransferFunctions { get; set; }

        public List<DatasetInfo> Datasets { get; set; }

        #endregion

        #region "Methods"

        public void Merge(VariableInfo variable)
        {
            if (this.Parent.Id != variable.Parent.Id)
                throw new Exception("The variable to be merged has a different parent.");

            // merge datasets
            List<DatasetInfo> newDatasets = new List<DatasetInfo>();

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

            // merge other properties
            this.VariableNames = this.MergeLists(this.VariableNames, variable.VariableNames);
            this.VariableGroups = this.MergeLists(this.VariableGroups, variable.VariableGroups);
            this.Units = this.MergeLists(this.Units, variable.Units);
            this.TransferFunctions = this.MergeLists(this.TransferFunctions, variable.TransferFunctions);
        }

        public string GetDisplayName()
        {
            return this.VariableNames.Last();
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

        private List<T> MergeLists<T>(List<T> a, List<T> b)
        {
            if (b.Count() == 0 || !Enumerable.SequenceEqual(b, a.Skip(Math.Max(0, a.Count() - b.Count()))))
                a = a.Concat(b).ToList();

            return a;
        }

        #endregion
    }
}
