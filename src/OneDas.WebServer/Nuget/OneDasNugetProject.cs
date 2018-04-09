using NuGet.ProjectManagement.Projects;

namespace OneDas.WebServer.Nuget
{
    public class OneDasNugetProject : ProjectJsonNuGetProject
    {
        public OneDasNugetProject(string projectFilePath) : base(projectFilePath, projectFilePath)
        {
            //
        }
    }   
}
