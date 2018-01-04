using OneDas.Infrastructure;
using System.Collections.Generic;
using OneDas.Plugin;

namespace OneDas.WebServer.Web
{
    public class AppModel
    {
        public AppModel(Project activeProject, IEnumerable<string> clientSet, IEnumerable<PluginIdentificationAttribute> dataGatewayPluginIdentificationSet, IEnumerable<PluginIdentificationAttribute> dataWriterPluginIdentificationSet, string lastError, OneDasState oneDasState, SlimOneDasSettings slimOneDasSettings)
        {
            this.ActiveProject = activeProject;
            this.ClientSet = clientSet;
            this.DataGatewayPluginIdentificationSet = dataGatewayPluginIdentificationSet;
            this.DataWriterPluginIdentificationSet = dataWriterPluginIdentificationSet;
            this.LastError = lastError;
            this.OneDasState = oneDasState;
            this.SlimOneDasSettings = slimOneDasSettings;
        }

        public readonly Project ActiveProject;
        public readonly IEnumerable<string> ClientSet;
        public readonly IEnumerable<PluginIdentificationAttribute> DataGatewayPluginIdentificationSet;
        public readonly IEnumerable<PluginIdentificationAttribute> DataWriterPluginIdentificationSet;
        public readonly string LastError;
        public readonly OneDasState OneDasState;
        public readonly SlimOneDasSettings SlimOneDasSettings;
    }
}
