using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.Serialization;

namespace OneDas.DataManagement.Explorer.Core
{
    [DataContract]
    public class OneDasExplorerOptions
    {
        public OneDasExplorerOptions()
        {
            // unset, mutable
            this.DataBaseFolderPath = string.Empty;
            this.InactiveOn = TimeSpan.Zero;
            this.InactivityPeriod = TimeSpan.Zero;

            // preset, mutable
            this.AggregationChunkSizeMB = 200;
            this.AggregationPeriodDays = 60;
            this.AspBaseUrl = "http://0.0.0.0:32769";
            this.Language = "en";
        }

        // unset, mutable
        [DataMember]
        public string DataBaseFolderPath { get; set; }

        [DataMember]
        public TimeSpan InactiveOn { get; set; }

        [DataMember]
        public TimeSpan InactivityPeriod { get; set; }

        // preset, mutable

        [DataMember]
        public uint AggregationChunkSizeMB { get; set; }

        [DataMember]
        public uint AggregationPeriodDays { get; set; }

        [DataMember]
        public string AspBaseUrl { get; set; }

        [DataMember]
        public string Language { get; set; }

        // preset, immutable
        public string ExportDirectoryPath { get => Path.Combine(this.DataBaseFolderPath, "EXPORT"); }

        public void Save(string directoryPath)
        {
            Directory.CreateDirectory(directoryPath);

            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(directoryPath, "settings.json")))
            {
                string rawJson;

                rawJson = JsonConvert.SerializeObject(this, Formatting.Indented);
                new JsonTextWriter(streamWriter).WriteRaw(rawJson);
            }
        }
    }
}
