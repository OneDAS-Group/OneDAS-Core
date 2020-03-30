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

        public bool TryFindDataset(string campaignName, string variableName, string datasetName, out DatasetInfo dataset)
        {
            dataset = default;

            var campaignContainer = this.CampaignContainers.FirstOrDefault(campaignContainer => campaignContainer.Name == campaignName);

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

        #endregion
    }
}