using OneDas.PackageManagement;
using System.Collections.Generic;

namespace OneDas.Core.WebClient.Model
{
    public class WebServerOptionsLight
    {
        public string OneDasName { get; set; }
        public string AspBaseUrl { get; set; }
        public string BaseDirectoryPath { get; set; }
        public List<OneDasPackageSource> PackageSourceSet { get; set; }
    }
}
