using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization;

namespace OneDas.WebServer
{
    [DataContract]
    public class WebServerOptions
    {
        public WebServerOptions()
        {
            // unset, mutable
            this.BaseDirectoryPath = string.Empty;
            this.CurrentProjectFilePath = string.Empty;
            this.NewBaseDirectoryPath = string.Empty;

            // preset, mutable
            this.AspBaseUrl = "http://0.0.0.0:32768";
            this.OneDasName = "OneDAS";

            // preset, immutable
            this.ConsoleHubName = "consolehub";
            this.EventLogName = "OneDAS";
            this.EventLogSourceName = "OneDAS Core";
            this.MutexName = "{fcacd9a4-dc61-4179-b92c-e11538aaf85b}";
            this.PluginPackageTypeName = "OneDasPlugin";
            this.ServiceName =  "OneDas.Core";
            this.WebClientHubName = "webclienthub";
        }
        
        // unset, mutable
        [DataMember]
        public string BaseDirectoryPath { get; set; }
        [DataMember]
        public string CurrentProjectFilePath { get; set; }
        [DataMember]
        public string NewBaseDirectoryPath { get; set; }

        // preset, mutable
        [DataMember]
        public string AspBaseUrl { get; set; }

        [DataMember]
        public string OneDasName { get; set; }

        // preset, immutable
        public string ConsoleHubName { get; private set; }
        public string EventLogName { get; private set; }
        public string EventLogSourceName { get; private set; }
        public string MutexName { get; private set; }
        public string PluginPackageTypeName { get; private set; }
        public string ServiceName { get; private set; }
        public string WebClientHubName { get; private set; }

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
