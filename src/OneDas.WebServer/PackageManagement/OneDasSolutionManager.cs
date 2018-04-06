using NuGet.PackageManagement;
using NuGet.ProjectManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneDas.WebServer.PackageManagement
{
    public class OneDasSolutionManager : ISolutionManager
    {
        NuGetProject _nuGetProject;

        public string SolutionDirectory
        {
            get;
        }

        public bool IsSolutionOpen
        {
            get;
        }

        public INuGetProjectContext NuGetProjectContext { get; set; }

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

        public OneDasSolutionManager(INuGetProjectContext nuGetProjectContext, NuGetProject nuGetProject, string projectDirectoryPath)
        {
            this.SolutionDirectory = projectDirectoryPath;
            this.NuGetProjectContext = nuGetProjectContext;
            _nuGetProject = nuGetProject;

            this.IsSolutionOpen = true;
        }

        public Task<bool> DoesNuGetSupportsAnyProjectAsync()
        {
            return Task.FromResult(true);
        }

        public void EnsureSolutionIsLoaded()
        {
            //
        }

        public Task<NuGetProject> GetNuGetProjectAsync(string nuGetProjectSafeName)
        {
            return Task.FromResult(_nuGetProject);
        }

        public Task<string> GetNuGetProjectSafeNameAsync(NuGetProject nuGetProject)
        {
            return Task.FromResult("YoYoYo");
        }

        public Task<IEnumerable<NuGetProject>> GetNuGetProjectsAsync()
        {
            IEnumerable<NuGetProject> list = new List<NuGetProject>() { _nuGetProject };

            return Task.FromResult(list);
        }

        public Task<bool> IsSolutionAvailableAsync()
        {
            return Task.FromResult(true);
        }

        public void OnActionsExecuted(IEnumerable<ResolvedAction> actions)
        {
            //
        }
    }
}
