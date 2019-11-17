using NuGet.PackageManagement;
using NuGet.ProjectManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneDas.PackageManagement
{
    public class OneDasSolutionManager : ISolutionManager
    {
        #region "Events"

#pragma warning disable 0067

        public event EventHandler SolutionOpening;
        public event EventHandler SolutionOpened;
        public event EventHandler SolutionClosing;
        public event EventHandler SolutionClosed;
        public event EventHandler<NuGetEventArgs<string>> AfterNuGetCacheUpdated;
        public event EventHandler<NuGetProjectEventArgs> NuGetProjectAdded;
        public event EventHandler<NuGetProjectEventArgs> NuGetProjectRemoved;
        public event EventHandler<NuGetProjectEventArgs> NuGetProjectRenamed;
        public event EventHandler<NuGetProjectEventArgs> NuGetProjectUpdated;
        public event EventHandler<NuGetProjectEventArgs> AfterNuGetProjectRenamed;
        public event EventHandler<ActionsExecutedEventArgs> ActionsExecuted;

#pragma warning restore 0067

        #endregion

        #region "Fields"

        NuGetProject _project;

        #endregion

        #region "Constructors"

        public OneDasSolutionManager(INuGetProjectContext projectContext, NuGetProject project, string projectDirectoryPath)
        {
            this.SolutionDirectory = projectDirectoryPath;
            this.NuGetProjectContext = projectContext;

            _project = project;

            this.IsSolutionOpen = true;
        }

        #endregion

        #region "Properties"

        public string SolutionDirectory { get; }

        public bool IsSolutionOpen { get; }

        public INuGetProjectContext NuGetProjectContext { get; set; }

        #endregion

        #region "Methods"

        public void EnsureSolutionIsLoaded()
        {
            //
        }

        public void OnActionsExecuted(IEnumerable<ResolvedAction> actions)
        {
            //
        }

        public Task<bool> DoesNuGetSupportsAnyProjectAsync()
        {
            return Task.FromResult(true);
        }

        public Task<NuGetProject> GetNuGetProjectAsync(string nuGetProjectSafeName)
        {
            return Task.FromResult(_project);
        }

        public Task<string> GetNuGetProjectSafeNameAsync(NuGetProject nuGetProject)
        {
            return Task.FromResult((string)_project.Metadata[NuGetProjectMetadataKeys.UniqueName]);
        }

        public Task<IEnumerable<NuGetProject>> GetNuGetProjectsAsync()
        {
            IEnumerable<NuGetProject> projectSet;

            projectSet = new List<NuGetProject>() { _project };

            return Task.FromResult(projectSet);
        }

        public Task<bool> IsSolutionAvailableAsync()
        {
            return Task.FromResult(true);
        }

        #endregion
    }
}
