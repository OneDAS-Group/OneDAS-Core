using OneDas.Core.Engine;
using OneDas.Core.ProjectManagement;
using OneDas.Extensibility;
using OneDas.PackageManagement;
using System.Collections.Generic;

namespace OneDas.WebServer.Web
{
    public class AppModel
    {
        public AppModel(OneDasProjectSettings activeProjectSettings, IEnumerable<OneDasPackageMetaData> installedPackageSet, IEnumerable<string> clientSet, IEnumerable<ExtensionIdentificationAttribute> dataGatewayExtensionIdentificationSet, IEnumerable<ExtensionIdentificationAttribute> dataWriterExtensionIdentificationSet, string productVersion, string lastError, OneDasState oneDasState, WebServerOptionsLight webServerOptionsLight)
        {
            this.ProductVersion = productVersion;
            this.InstalledPackageSet = installedPackageSet;
            this.ActiveProjectSettings = activeProjectSettings;
            this.ClientSet = clientSet;
            this.DataGatewayExtensionIdentificationSet = dataGatewayExtensionIdentificationSet;
            this.DataWriterExtensionIdentificationSet = dataWriterExtensionIdentificationSet;
            this.LastError = lastError;
            this.OneDasState = oneDasState;
            this.WebServerOptionsLight = webServerOptionsLight;
        }

        // if this list is modified, remember to update method "Update()" in AppViewModel.ts
        public readonly OneDasProjectSettings ActiveProjectSettings;
        public readonly IEnumerable<OneDasPackageMetaData> InstalledPackageSet;
        public readonly IEnumerable<string> ClientSet;
        public readonly IEnumerable<ExtensionIdentificationAttribute> DataGatewayExtensionIdentificationSet;
        public readonly IEnumerable<ExtensionIdentificationAttribute> DataWriterExtensionIdentificationSet;
        public readonly string ProductVersion;
        public readonly string LastError;
        public readonly OneDasState OneDasState;
        public readonly WebServerOptionsLight WebServerOptionsLight;
    }
}
