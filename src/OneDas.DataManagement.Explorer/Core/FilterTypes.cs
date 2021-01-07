using OneDas.DataManagement.Explorer.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace OneDas.DataManagement.Explorer.Core
{
    public enum CodeType
    {
        Filter = 1,
        Shared = 99
    }

    public enum CodeLanguage
    {
        CSharp = 1
    }

    public record CodeDefinition()
    {
        public CodeDefinition(string owner) : this()
        {
            this.Owner = owner;
        }

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Owner { get; set; }

        public CodeType CodeType { get; set; } = CodeType.Filter;

        public CodeLanguage CodeLanguage { get; set; } = CodeLanguage.CSharp;

        public string Code { get; set; } = string.Empty;

        public bool IsPublic { get; set; }

        public string Name { get; set; } = string.Empty;

        public string SampleRate { get; set; }

        public List<string> RequestedProjectIds { get; set; } = new List<string>();

        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    }

    public record FilterSettings
    {
        #region Constructors

        public FilterSettings()
        {
            this.CodeDefinitions = new List<CodeDefinition>();
        }

        #endregion

        #region Properties

        public List<CodeDefinition> CodeDefinitions { get; set; }

        #endregion

        #region Methods

        public List<CodeDefinition> GetSharedFiles(string userName)
        {
            return this.CodeDefinitions
                   .Where(codeDefinition =>
                          codeDefinition.Owner == userName &&
                          codeDefinition.CodeType == CodeType.Shared)
                   .ToList();
        }

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

    public static class FilterChannelExtensions
    {
        #region Methods

        public static Guid ToGuid(this FilterChannel filterChannel)
        {
            // With the filter extension, channels are produced dynamically, so there
            // is no way to generate an ID once and store it somewhere. Therefore the 
            // ID must be generated each time the filter code is instantiated. To avoid 
            // having ever changing IDs, the ID is effectively a good hash code based 
            // on the project ID and channel name. In the end this means that the 
            // channel name determines the ID. And so renaming a channel means changing 
            // the ID.
            var value = $"{filterChannel.ProjectId}/{filterChannel.ChannelName}";
            var md5 = MD5.Create(); // compute hash is not thread safe!
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(value)); // 
            return new Guid(hash);
        }

        #endregion
    }
}
