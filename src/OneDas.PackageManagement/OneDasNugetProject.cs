using NuGet.ProjectManagement.Projects;

namespace OneDas.PackageManagement
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
