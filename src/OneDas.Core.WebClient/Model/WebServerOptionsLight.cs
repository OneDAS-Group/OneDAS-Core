using OneDas.PackageManagement;
using System.Collections.Generic;

namespace OneDas.Core.WebClient.Model
{
    public class WebServerOptionsLight
    {
        public string OneDasName;
        public string AspBaseUrl;
        public string BaseDirectoryPath;
        public List<OneDasPackageSource> PackageSourceSet;
    }
}
