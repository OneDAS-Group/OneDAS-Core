using OneDas.PackageManagement;
using System.Collections.Generic;

namespace OneDas.WebServer.Web
{
    public class WebServerOptionsLight
    {
        public string OneDasName;
        public string AspBaseUrl;
        public string BaseDirectoryPath;
        public List<OneDasPackageSource> PackageSourceSet;
    }
}
