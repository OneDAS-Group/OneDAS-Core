using HDF.PInvoke;
using OneDas.DataManagement.Hdf;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace OneDas.DataManagement.Database
{
    public class HdfVariableInfo : CampaignElement
    {
        #region "Constructors"

        public HdfVariableInfo(string id, CampaignElement parent) : base(id, parent)
        {
            this.VariableNames = new List<string>();
            this.VariableGroups = new List<string>();
            this.Units = new List<string>();
            this.TransferFunctions = new List<TransferFunction>();
            this.Datasets = new List<HdfDatasetInfo>();
        }

        #endregion

        #region "Properties"

        public List<string> VariableNames { get; private set; }

        public List<string> VariableGroups { get; private set; }

        public List<string> Units { get; private set; }

        public List<TransferFunction> TransferFunctions { get; private set; }

        public List<HdfDatasetInfo> Datasets { get; }

        #endregion

        #region "Methods"

        public VariableInfo ToVariable(CampaignInfo parent)
        {
            var variable = new VariableInfo(this.Id, parent)
            {
                Name = this.VariableNames.LastOrDefault(),
                Group = this.VariableGroups.LastOrDefault(),
                Unit = this.Units.LastOrDefault(),
                TransferFunctions = this.TransferFunctions
            };

            variable.Datasets = this.Datasets.Select(dataset => dataset.ToDataset(variable)).ToList();

            return variable;
        }

        public void Update(long variableGroupId, FileContext fileContext, UpdateSourceFileMapDelegate updateSourceFileMap)
        {
            var idx = 0UL;

            this.VariableNames = IOHelper.UpdateAttributeList(variableGroupId, "name_set", this.VariableNames.ToArray()).ToList();
            this.VariableGroups = IOHelper.UpdateAttributeList(variableGroupId, "group_set", this.VariableGroups.ToArray()).ToList();

            if (fileContext.FormatVersion != 1)
            {
                this.Units = IOHelper.UpdateAttributeList(variableGroupId, "unit_set", this.Units.ToArray()).ToList();

                // TransferFunction to hdf_transfer_function_t
                var transferFunctionSet = this.TransferFunctions.Select(tf => hdf_transfer_function_t.FromTransferFunction(tf));

                // hdf_transfer_function_t to TransferFunction
                this.TransferFunctions = IOHelper.UpdateAttributeList(variableGroupId, "transfer_function_set", transferFunctionSet.ToArray())
                    .Select(tf => tf.ToTransferFunction())
                    .ToList();
            }

            H5L.iterate(variableGroupId, H5.index_t.NAME, H5.iter_order_t.INC, ref idx, Callback, IntPtr.Zero);

            int Callback(long variableGroupId2, IntPtr intPtrName, ref H5L.info_t info, IntPtr userDataPtr)
            {
                long datasetId = -1;

                string name;

                HdfDatasetInfo currentDataset;

                try
                {
                    name = Marshal.PtrToStringAnsi(intPtrName);

                    if (H5L.exists(variableGroupId2, name) > 0)
                    {
                        datasetId = H5D.open(variableGroupId2, name);

                        currentDataset = this.Datasets.FirstOrDefault(dataset => dataset.Id == name);

                        if (currentDataset == null)
                        {
                            currentDataset = new HdfDatasetInfo(name, this);
                            this.Datasets.Add(currentDataset);
                        }

                        currentDataset.Update(datasetId, fileContext, updateSourceFileMap);
                    }
                }
                finally
                {
                    if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
                }

                return 0;
            }
        }

        public override string GetPath()
        {
            return $"{this.Parent.GetPath()}/{this.Id}";
        }

        public override IEnumerable<CampaignElement> GetChilds()
        {
            return this.Datasets;
        }

        #endregion
    }
}
