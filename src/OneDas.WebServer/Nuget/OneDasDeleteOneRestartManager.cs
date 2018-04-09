#pragma warning disable 67

using NuGet.PackageManagement;
using NuGet.Packaging.Core;
using NuGet.ProjectManagement;
using System;
using System.Collections.Generic;

namespace OneDas.WebServer.Nuget
{
    public class OneDasDeleteOnRestartManager : IDeleteOnRestartManager
    {
        public event EventHandler<PackagesMarkedForDeletionEventArgs> PackagesMarkedForDeletionFound;

        public void CheckAndRaisePackageDirectoriesMarkedForDeletion()
        {
            //
        }

        public void DeleteMarkedPackageDirectories(INuGetProjectContext projectContext)
        {
            //
        }

        public IReadOnlyList<string> GetPackageDirectoriesMarkedForDeletion()
        {
            return new List<string>();
        }

        public void MarkPackageDirectoryForDeletion(PackageIdentity package, string packageDirectory, INuGetProjectContext projectContext)
        {
            //
        }
    }
}
