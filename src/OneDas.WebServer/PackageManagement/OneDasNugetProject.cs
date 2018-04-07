using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.ProjectManagement;
using NuGet.ProjectManagement.Projects;
using NuGet.ProjectModel;
using NuGet.Versioning;

namespace OneDas.WebServer.PackageManagement
{
    public class OneDasNugetProject : ProjectJsonNuGetProject
    {
        public OneDasNugetProject(string jsonConfig) : base(jsonConfig, jsonConfig)
        {
            //
        }
    }   
}
