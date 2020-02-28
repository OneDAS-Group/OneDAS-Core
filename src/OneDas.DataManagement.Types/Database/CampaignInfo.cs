using OneDas.DataStorage;
using OneDas.Extensibility;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OneDas.DataManagement.Database
{
    [DebuggerDisplay("{Name,nq}")]
    public class CampaignInfo : CampaignElement
    {
        #region "Constructors"

        public CampaignInfo(string name) : base(name, null)
        {
#warning Remove "is_chunk_completed_set"
            this.ChunkDataset = new DatasetInfo("is_chunk_completed_set", this);
            this.Variables = new List<VariableInfo>();
        }

        private CampaignInfo()
        {
            //
        }

        #endregion

        #region "Properties"

        public DateTime CampaignStart { get; set; }

        public DateTime CampaignEnd { get; set; }

        public DatasetInfo ChunkDataset { get; set; }

        public List<VariableInfo> Variables { get; set; }

        #endregion

        #region "Methods"

        public List<VariableDescription> ToVariableDescriptions()
        {
            return this.Variables.SelectMany(variable =>
            {
                return variable.Datasets.Select(dataset =>
                {
                    var guid = new Guid(variable.Name);
                    var displayName = variable.VariableNames.Last();
                    var datasetName = dataset.Name;
                    var groupName = variable.VariableGroups.Any() ? variable.VariableGroups.Last() : string.Empty;
                    var dataType = dataset.DataType;
                    var sampleRate = new SampleRateContainer(dataset.Name);
                    var unit = variable.Units.Any() ? variable.Units.Last() : string.Empty;
                    var transferFunctions = variable.TransferFunctions;

                    return new VariableDescription(guid, displayName, datasetName, groupName, dataType, sampleRate, unit, transferFunctions, DataStorageType.Simple);
                });
            }).ToList();
        }

        public CampaignInfo ToSparseCampaign(Dictionary<string, List<string>> variableMap)
        {
            var campaign = new CampaignInfo(this.Name);

            campaign.Variables = variableMap.Select(variableEntry =>
            {
                var referenceVariable = this.Variables.FirstOrDefault(currentVariable => currentVariable.Name == variableEntry.Key);

                if (referenceVariable is null)
                    throw new KeyNotFoundException($"The requested variable '{referenceVariable.Name}' is unknown.");

                var variable = new VariableInfo(referenceVariable.Name, campaign)
                {
                    TransferFunctions = referenceVariable.TransferFunctions,
                    Units = referenceVariable.Units,
                    VariableGroups = referenceVariable.VariableGroups,
                    VariableNames = referenceVariable.VariableNames
                };

                variable.Datasets = variableEntry.Value.Select(datasetName =>
                {
                    var referenceDataset = referenceVariable.Datasets.FirstOrDefault(currentDataset => currentDataset.Name == datasetName);

                    if (referenceDataset is null)
                        throw new KeyNotFoundException($"The requested dataset '{referenceDataset.Name}' is unknown.");

                    return new DatasetInfo(referenceDataset.Name, variable) 
                    { 
                        DataType = referenceDataset.DataType,
                        IsNative = referenceDataset.IsNative
                    };
                }).ToList();

                return variable;
            }).ToList();

            return campaign;
        }

        public void Merge(CampaignInfo campaign)
        {
            if (this.Name != campaign.Name)
                throw new Exception("The campaign to be merged has a different name.");

            // merge variables
            List<VariableInfo> newVariables = new List<VariableInfo>();

            foreach (var variable in campaign.Variables)
            {
                var referenceVariable = this.Variables.FirstOrDefault(current => current.Name == variable.Name);

                if (referenceVariable != null)
                    referenceVariable.Merge(variable);
                else
                    newVariables.Add(variable);
            }

            this.Variables.AddRange(newVariables);

            // merge other data
            if (this.CampaignStart == DateTime.MinValue)
                this.CampaignStart = campaign.CampaignStart;
            else
                this.CampaignStart = new DateTime(Math.Min(this.CampaignStart.Ticks, campaign.CampaignStart.Ticks));

            if (this.CampaignEnd == DateTime.MinValue)
                this.CampaignEnd = campaign.CampaignEnd;
            else
                this.CampaignEnd = new DateTime(Math.Max(this.CampaignEnd.Ticks, campaign.CampaignEnd.Ticks));
        }

        public override string GetPath()
        {
            return this.Name;
        }

        public override IEnumerable<CampaignElement> GetChilds()
        {
            return this.Variables;
        }

        public override void Initialize()
        {
            base.Initialize();

            this.ChunkDataset.Parent = this;

            foreach (var variable in this.Variables)
            {
                variable.Parent = this;
                variable.Initialize();
            }
        }

        #endregion
    }
}
