using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.Serialization;

namespace OneDas.DataManagement.Explorer
{
    [DataContract]
    public class HdfExplorerOptions
    {
        public HdfExplorerOptions()
        {
            // unset, mutable
            this.DataBaseFolderPath = string.Empty;
            this.InactiveOn = TimeSpan.Zero;
            this.InactivityPeriod = TimeSpan.Zero;

            // preset, mutable
            this.AspBaseUrl = "http://0.0.0.0:32769";
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
        public string AspBaseUrl { get; set; }

        // preset, immutable
        public string SupportDirectoryPath { get => Path.Combine(this.DataBaseFolderPath, "SUPPORT"); }

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
