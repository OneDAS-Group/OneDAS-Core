using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace OneDas.DataManagement.Explorer.Core
{
    public enum CodeType
    {
        Channel = 1,
        Project = 2,
        Shared = 3
    }

    public enum CodeLanguage
    {
        CSharp = 1
    }

    public record FilterDescription()
    {
        public FilterDescription(string owner) : this()
        {
            this.Owner = owner;
        }

        public String Owner { get; set; }

        public CodeType CodeType { get; set; } = CodeType.Channel;

        public CodeLanguage CodeLanguage { get; set; } = CodeLanguage.CSharp;

        public string Code { get; set; } = string.Empty;

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public bool IsPublic { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Group { get; set; } = string.Empty;

        public string Unit { get; set; } = string.Empty;

        public string SampleRate { get; set; }

        public List<string> RequestedProjectIds { get; set; } = new List<string>() { "/IN_MEMORY/TEST/ACCESSIBLE" };

        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
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
