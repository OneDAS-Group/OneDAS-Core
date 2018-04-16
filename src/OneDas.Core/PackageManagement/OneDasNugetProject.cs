using NuGet.ProjectManagement.Projects;

namespace OneDas.Core.PackageManagement
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
