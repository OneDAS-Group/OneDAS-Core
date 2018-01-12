using Newtonsoft.Json;
using OneDas.Engine.Serialization;
using System.IO;

namespace OneDas.WebServer
{
    public class WebServerOptions
    {
        public WebServerOptions()
        {
            this.AspBaseUrl = "http://127.0.0.1:32768";
            this.IsAnonymousAuthenticationEnabled = false;
            this.IsAutostartEnabled = false;
            this.OneDasName = "Prototype";

            this.AdGroupName = "Benutzer";
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
        public string BaseDirectoryPath { get; set; }
        public string CurrentProjectFilePath { get; set; }
        public string NewBaseDirectoryPath { get; set; }

        // preset, mutable
        public string AspBaseUrl { get; set; }
        public bool IsAnonymousAuthenticationEnabled { get; set; }
        public bool IsAutostartEnabled { get; set; }
        public string OneDasName { get; set; }

        // preset, immutable
        public string AdGroupName { get; private set; }
        public string ApplicationName { get; private set; }
        public string ApplicationDisplayName { get; private set; }
        public string ConsoleHubName { get; private set; }
        public string EventLogName { get; private set; }
        public string EventLogSourceName { get; private set; }
        public string MutexName { get; private set; }
        public string ServiceName { get; private set; }
        public string WebClientHubName { get; private set; }

        public void Save()
        {
            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(this.BaseDirectoryPath, "onedassettings.json")))
            {
                string rawJson;

                rawJson = JsonConvert.SerializeObject(this, Formatting.Indented, SerializationHelper.CreateDefaultSerializationSettings());
                new JsonTextWriter(streamWriter).WriteRaw(rawJson);
            }
        }
    }
}
