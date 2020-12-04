using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace OneDas.DataManagement.Explorer.Core
{
    public enum CodeLanguage
    {
        CSharp = 1
    }

    public record FilterDescription
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; } = string.Empty;

        public string Group { get; set; } = string.Empty;

        public string Unit { get; set; } = string.Empty;

        public string SampleRate { get; set; }

        public string Code { get; set; } = string.Empty;

        public CodeLanguage CodeLanguage { get; set; } = CodeLanguage.CSharp;

        public bool IsPublic { get; set; }
    }

    public record FilterSettings
    {
        #region Constructors

        public FilterSettings()
        {
            this.FilterDescriptions = new List<FilterDescription>();
        }

        #endregion

        #region Properties

        public List<FilterDescription> FilterDescriptions { get; set; }

        #endregion

        #region Methods

        public static FilterSettings Load(string filePath)
        {
            var jsonString = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<FilterSettings>(jsonString);
        }

        public void Save(string filePath)
        {
            var options = new JsonSerializerOptions() { WriteIndented = true };
            var jsonString = JsonSerializer.Serialize(this, options);
            File.WriteAllText(filePath, jsonString);
        }

        #endregion
    }
}
