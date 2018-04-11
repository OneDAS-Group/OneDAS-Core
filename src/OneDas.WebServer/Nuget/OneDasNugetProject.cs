using NuGet.ProjectManagement.Projects;

namespace OneDas.WebServer.Nuget
{
    public class OneDasNugetProject : ProjectJsonNuGetProject
    {
        #region "Constructors"

        public OneDasNugetProject(string projectFilePath) : base(projectFilePath, projectFilePath)
        {
            //
        }

        #endregion
    }
}
