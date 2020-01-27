using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace OneDas.Data
{
    [DebuggerDisplay("{GetDisplayName(),nq}")]
    [DataContract]
    public class VariableInfo : InfoBase
    {
#warning Ensure the properties are never zero (right now they may be set from outside)
        #region "Constructors"

        public VariableInfo(string name, InfoBase parent) : base(name, parent)
        {
            this.VariableNames = new List<string>();
            this.VariableGroups = new List<string>();
            this.Units = new List<string>();
            this.TransferFunctions = new List<TransferFunction>();

            this.Datasets = new List<DatasetInfo>();
        }

        #endregion

        #region "Properties"

        [DataMember]
        public List<string> VariableNames { get; set; }

        [DataMember]
        public List<string> VariableGroups { get; set; }

        [DataMember]
        public List<string> Units { get; set; }

        [DataMember]
        public List<TransferFunction> TransferFunctions { get; set; }

        [DataMember]
        public List<DatasetInfo> Datasets { get; set; }
        public object Variables { get; private set; }

        #endregion

        #region "Methods"

        public void Merge(VariableInfo variable)
        {
            if (this.Parent.Name != variable.Parent.Name)
                throw new Exception("The variable to be merged has a different parent.");

            // merge datasets
            List<DatasetInfo> newDatasets = new List<DatasetInfo>();

            foreach (var dataset in variable.Datasets)
            {
                var referenceDataset = this.Datasets.FirstOrDefault(current => current.Name == variable.Name);

                if (referenceDataset != null)
                    referenceDataset.Merge(dataset);
                else
                    newDatasets.Add(dataset);
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
            return $"{this.Parent.GetPath()}/{this.Name}";
        }

        public override IEnumerable<InfoBase> GetChilds()
        {
            return this.Datasets;
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
