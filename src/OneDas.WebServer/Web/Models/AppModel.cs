using OneDas.Engine.Core;
using OneDas.Infrastructure;
using OneDas.Plugin;
using System.Collections.Generic;

namespace OneDas.WebServer.Web
{
    public class AppModel
    {
        public AppModel(OneDasProjectSettings activeProjectSettings, IEnumerable<string> clientSet, IEnumerable<PluginIdentificationAttribute> dataGatewayPluginIdentificationSet, IEnumerable<PluginIdentificationAttribute> dataWriterPluginIdentificationSet, string lastError, OneDasState oneDasState, WebServerOptionsLight webServerOptionsLight)
        {
            this.ActiveProjectSettings = activeProjectSettings;
            this.ClientSet = clientSet;
            this.DataGatewayPluginIdentificationSet = dataGatewayPluginIdentificationSet;
            this.DataWriterPluginIdentificationSet = dataWriterPluginIdentificationSet;
            this.LastError = lastError;
            this.OneDasState = oneDasState;
            this.WebServerOptionsLight = webServerOptionsLight;
        }

        public readonly OneDasProjectSettings ActiveProjectSettings;
        public readonly IEnumerable<string> ClientSet;
        public readonly IEnumerable<PluginIdentificationAttribute> DataGatewayPluginIdentificationSet;
        public readonly IEnumerable<PluginIdentificationAttribute> DataWriterPluginIdentificationSet;
        public readonly string LastError;
        public readonly OneDasState OneDasState;
        public readonly WebServerOptionsLight WebServerOptionsLight;
    }
}
