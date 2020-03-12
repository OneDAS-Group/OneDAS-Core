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

        public bool TryFindDataset(string campaignName, string variableName, string datasetName, out DatasetInfo dataset)
        {
            dataset = default;

            var campaignContainer = this.CampaignContainers.FirstOrDefault(campaignContainer => campaignContainer.Name == campaignName);

            if (campaignContainer != null)
            {
                var variable = campaignContainer.Campaign.Variables.FirstOrDefault(variable => variable.Name == variableName);

                if (variable != null)
                {
                    dataset = variable.Datasets.FirstOrDefault(dataset => dataset.Name == datasetName);

                    if (dataset != null)
                        return true;
                }
            }

            return false;
        }

        #endregion
    }
}