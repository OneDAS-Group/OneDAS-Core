using OneDas.Extensibility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace OneDas.DataManagement.Convert
{
    public class OneDasCampaignContext
    {
        #region Constructors

        private OneDasCampaignContext()
        {
            //
        }

        private OneDasCampaignContext(string campaignName)
        {
            this.CampaignName = campaignName;
            this.CampaignGuid = Guid.NewGuid();
            this.VariableToGuidMap = new Dictionary<string, Guid>();
        }

        #endregion

        #region Properties

        public string CampaignName { get; set; }

        public Guid CampaignGuid { get; set; }

        public Dictionary<string, Guid> VariableToGuidMap { get; set; }

        #endregion

        #region Methods

        public static OneDasCampaignContext Update(string importDirectoryPath, string campaignName, List<VariableDescription> variableDescriptionSet)
        {
            var filePath = Path.Combine(importDirectoryPath, $"{campaignName.Replace('/', '_')}.json");

            OneDasCampaignContext convertContext;

            if (File.Exists(filePath))
                convertContext = JsonSerializer.Deserialize<OneDasCampaignContext>(File.ReadAllText(filePath));
            else
                convertContext = new OneDasCampaignContext(campaignName);

            convertContext.AddVariables(variableDescriptionSet);

            // save file
            var options = new JsonSerializerOptions { WriteIndented = true };
            var writeJsonString = JsonSerializer.Serialize(convertContext, options);

            File.WriteAllText(filePath, writeJsonString);

            return convertContext;
        }

        private void AddVariables(List<VariableDescription> variableDescriptionSet)
        {
            foreach (var variableDescription in variableDescriptionSet)
            {
                if (!this.VariableToGuidMap.ContainsKey(variableDescription.VariableName))
                    this.VariableToGuidMap[variableDescription.VariableName] = Guid.NewGuid();
            }
        }

        #endregion
    }
}
