using Newtonsoft.Json;
using OneDas.Engine.Serialization;
using System.IO;
using System.Runtime.Serialization;

namespace OneDas.WebServer
{
    [DataContract]
    public class WebServerOptions
    {
        public WebServerOptions()
        {
            // preset, mutable
            this.AspBaseUrl = "http://127.0.0.1:32768";
            this.OneDasName = "Prototype";

            // preset, immutable
            this.ApplicationName = "OneDAS WebServer";
            this.ApplicationDisplayName = "OneDas.WebServer";
            this.ConsoleHubName = "consolehub";
            this.EventLogName = "OneDAS";
            this.EventLogSourceName = this.ApplicationName;
            this.MutexName = "{fcacd9a4-dc61-4179-b92c-e11538aaf85b}";
            this.ServiceName = this.ApplicationDisplayName;
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
        public string ApplicationName { get; private set; }
        public string ApplicationDisplayName { get; private set; }
        public string ConsoleHubName { get; private set; }
        public string EventLogName { get; private set; }
        public string EventLogSourceName { get; private set; }
        public string MutexName { get; private set; }
        public string ServiceName { get; private set; }
        public string WebClientHubName { get; private set; }

        public void Save(string directoryPath)
        {
            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(directoryPath, "onedassettings.json")))
            {
                string rawJson;

                rawJson = JsonConvert.SerializeObject(this, Formatting.Indented);
                new JsonTextWriter(streamWriter).WriteRaw(rawJson);
            }
        }
    }
}
