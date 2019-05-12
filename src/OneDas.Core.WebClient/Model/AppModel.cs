using OneDas.Extensibility;
using OneDas.PackageManagement;
using System.Collections.Generic;

namespace OneDas.Core.WebClient.Model
{
    public class AppModel
    {
        //public readonly OneDasProjectSettings ActiveProjectSettings;
        public IEnumerable<OneDasPackageMetaData> InstalledPackageSet { get; set; }
        public IEnumerable<string> ClientSet { get; set; }
        public IEnumerable<ExtensionIdentificationAttribute> DataGatewayExtensionIdentificationSet { get; set; }
        public IEnumerable<ExtensionIdentificationAttribute> DataWriterExtensionIdentificationSet { get; set; }
        public string ProductVersion { get; set; }
        public string LastError { get; set; }
        public OneDasState OneDasState { get; set; }
        //public WebServerOptionsLight WebServerOptionsLight { get; set; }
    }
}
