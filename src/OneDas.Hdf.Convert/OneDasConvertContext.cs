using OneDas.Extensibility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace OneDas.Hdf.Convert
{
    public class OneDasConvertContext
    {
        #region Constructors

        private OneDasConvertContext()
        {
            //
        }

        private OneDasConvertContext(string campaignName)
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

        public static OneDasConvertContext OpenOrCreate(string importDirectoryPath, string campaignName, List<VariableDescription> variableDescriptionSet)
        {
            var filePath = Path.Combine(importDirectoryPath, $"{campaignName.Replace('/', '_')}.json");

            OneDasConvertContext importContext;

            if (File.Exists(filePath))
                importContext = JsonSerializer.Deserialize<OneDasConvertContext>(File.ReadAllText(filePath));
            else
                importContext = new OneDasConvertContext(campaignName);

            importContext.AddVariables(variableDescriptionSet);

            // save file
            var options = new JsonSerializerOptions { WriteIndented = true };
            var writeJsonString = JsonSerializer.Serialize(importContext, options);

            File.WriteAllText(filePath, writeJsonString);

            return importContext;
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
