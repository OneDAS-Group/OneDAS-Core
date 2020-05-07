using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OneDas.DataManagement.Database
{
    [DebuggerDisplay("{Name,nq}")]
    public class CampaignContainer
    {
        #region "Constructors"

        public CampaignContainer(string id, string rootPath)
        {
            this.Id = id;
            this.RootPath = rootPath;
            this.Campaign = new CampaignInfo(id);
        }

        private CampaignContainer()
        {
            //
        }

        #endregion

        #region "Properties"

        public string Id { get; set; }

        public string PhysicalName => this.Id.TrimStart('/').Replace('/', '_');

        public string RootPath { get; set; }

        public CampaignInfo Campaign { get; set; }

        public CampaignMetaInfo CampaignMeta { get; set; }

        #endregion

        #region Methods

        public void Initialize()
        {
            this.Campaign.Initialize();
        }

        public SparseCampaignInfo ToSparseCampaign(List<DatasetInfo> datasets)
        {
            var campaign = new SparseCampaignInfo(this.Id);
            var variables = datasets.Select(dataset => (VariableInfo)dataset.Parent).Distinct().ToList();

            campaign.Variables = variables.Select(reference =>
            {
                var variableMeta = this.CampaignMeta.Variables.First(variableMeta => variableMeta.Id == reference.Id);

                var variable = new VariableInfo(reference.Id, campaign)
                {
                    Name = reference.Name,
                    Group = reference.Group,

                    Unit = !string.IsNullOrWhiteSpace(variableMeta.Unit) 
                        ? variableMeta.Unit
                        : reference.Unit,

                    TransferFunctions = variableMeta.TransferFunctions.Any()
                        ? variableMeta.TransferFunctions
                        : reference.TransferFunctions
                };

                var referenceDatasets = datasets.Where(dataset => (VariableInfo)dataset.Parent == reference);

                variable.Datasets = referenceDatasets.Select(referenceDataset =>
                {
                    return new DatasetInfo(referenceDataset.Id, variable)
                    {
                        DataType = referenceDataset.DataType,
                        IsNative = referenceDataset.IsNative
                    };
                }).ToList();

                return variable;
            }).ToList();

            return campaign;
        }

        #endregion
    }
}