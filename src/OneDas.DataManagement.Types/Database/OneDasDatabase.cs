using System;
using System.Collections.Generic;
using System.Linq;

namespace OneDas.DataManagement.Database
{
    public class OneDasDatabase
    {
        #region Constructors

        public OneDasDatabase()
        {
            this.CampaignContainers = new List<CampaignContainer>();
        }

        #endregion

        #region Properties

        public List<CampaignContainer> CampaignContainers { get; set; }

        #endregion

        #region Methods

        public List<CampaignInfo> GetCampaigns()
        {
            return this.CampaignContainers.Select(container => container.Campaign).ToList();
        }

        public bool TryFindCampaignById(string id, out CampaignInfo campaign)
        {
            var campaignContainer = this.CampaignContainers.FirstOrDefault(campaignContainer => campaignContainer.Id == id);
            campaign = campaignContainer?.Campaign;

            return campaign != null;
        }

        public bool TryFindVariableById(string campaignId, string variableId, out VariableInfo variable)
        {
            variable = default;

            if (this.TryFindCampaignById(campaignId, out var campaign))
            {
                variable = campaign.Variables.FirstOrDefault(variable => variable.Id == variableId);

                if (variable == null)
                    variable = campaign.Variables.FirstOrDefault(variable => variable.Name == variableId);
            }

            return variable != null;
        }

        public bool TryFindDatasetById(string campaignId, string variableId, string datsetId, out DatasetInfo dataset)
        {
            dataset = default;

            if (this.TryFindVariableById(campaignId, variableId, out var variable))
            {
                dataset = variable.Datasets.FirstOrDefault(dataset => dataset.Id == datsetId);

                if (dataset != null)
                    return true;
            }

            return false;
        }

        public bool TryFindDataset(string path, out DatasetInfo dataset)
        {
            var pathParts = path.Split("/");

            if (pathParts.Length != 6)
                throw new Exception($"The channel name '{path}' is invalid.");

            var campaignName = $"/{pathParts[1]}/{pathParts[2]}/{pathParts[3]}";
            var variableName = pathParts[4];
            var datasetName = pathParts[5];

            return this.TryFindDatasetById(campaignName, variableName, datasetName, out dataset);
        }

        public bool TryFindDatasetsByGroup(string groupPath, out List<DatasetInfo> datasets)
        {
            var groupPathParts = groupPath.Split("/");

            if (groupPathParts.Length != 6)
                throw new Exception($"The group path '{groupPath}' is invalid.");

            var campaignName = $"/{groupPathParts[1]}/{groupPathParts[2]}/{groupPathParts[3]}";
            var groupName = groupPathParts[4];
            var datasetName = groupPathParts[5];

            return this.TryFindDatasetsByGroup(campaignName, groupName, datasetName, out datasets);
        }

        private bool TryFindDatasetsByGroup(string campaignName, string groupName, string datasetName, out List<DatasetInfo> datasets)
        {
            datasets = new List<DatasetInfo>();

            var campaignContainer = this.CampaignContainers.FirstOrDefault(campaignContainer => campaignContainer.Id == campaignName);

            if (campaignContainer != null)
            {
                var variables = campaignContainer.Campaign.Variables
                    .Where(variable => variable.Group.Split('\n')
                    .Contains(groupName))
                    .OrderBy(variable => variable.Name)
                    .ToList();

                datasets
                    .AddRange(variables
                    .SelectMany(variable => variable.Datasets
                    .Where(dataset => dataset.Id == datasetName)));

                if (datasets.Any())
                    return true;
            }

            return false;
        }

        #endregion
    }
}