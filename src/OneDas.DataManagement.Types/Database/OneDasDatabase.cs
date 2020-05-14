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

        public bool TryFindDataset(string channelName, out DatasetInfo dataset)
        {
            var channelNameParts = channelName.Split("/");

            if (channelNameParts.Length != 6)
                throw new Exception($"The channel name '{channelName}' is invalid.");

            var campaignName = $"/{channelNameParts[1]}/{channelNameParts[2]}/{channelNameParts[3]}";
            var variableName = channelNameParts[4];
            var datasetName = channelNameParts[5];

            return this.TryFindDataset(campaignName, variableName, datasetName, out dataset);
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

        private bool TryFindDataset(string campaignName, string variableName, string datasetName, out DatasetInfo dataset)
        {
            dataset = default;

            var campaignContainer = this.CampaignContainers.FirstOrDefault(campaignContainer => campaignContainer.Id == campaignName);

            if (campaignContainer != null)
            {
                var variable = campaignContainer.Campaign.Variables.FirstOrDefault(variable => variable.Id == variableName);

                if (variable == null)
                    variable = campaignContainer.Campaign.Variables.FirstOrDefault(variable => variable.Name == variableName);

                if (variable != null)
                {
                    dataset = variable.Datasets.FirstOrDefault(dataset => dataset.Id == datasetName);

                    if (dataset != null)
                        return true;
                }
            }

            return false;
        }

        private bool TryFindDatasetsByGroup(string campaignName, string groupName, string datasetName, out List<DatasetInfo> datasets)
        {
            datasets = new List<DatasetInfo>();

            var campaignContainer = this.CampaignContainers.FirstOrDefault(campaignContainer => campaignContainer.Id == campaignName);

            if (campaignContainer != null)
            {
                var variables = campaignContainer.Campaign.Variables.Where(variable => variable.Group == groupName);

                datasets
                    .AddRange(variables
                    .SelectMany(variable => variable.Datasets
                    .Where(dataset => dataset.Id == datasetName)));

                if (!datasets.Any())
                    return false;
            }

            return false;
        }

        #endregion
    }
}