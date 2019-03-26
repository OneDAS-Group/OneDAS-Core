using NuGet.PackageManagement;
using NuGet.Packaging.Core;
using NuGet.ProjectManagement;
using System;
using System.Collections.Generic;

namespace OneDas.PackageManagement
{
    public class OneDasDeleteOnRestartManager : IDeleteOnRestartManager
    {
#pragma warning disable 0067

        public event EventHandler<PackagesMarkedForDeletionEventArgs> PackagesMarkedForDeletionFound;

#pragma warning restore 0067

        public void CheckAndRaisePackageDirectoriesMarkedForDeletion()
        {
            //
        }

        public void DeleteMarkedPackageDirectories(INuGetProjectContext projectContext)
        {
            //
        }

        public void MarkPackageDirectoryForDeletion(PackageIdentity package, string packageDirectory, INuGetProjectContext projectContext)
        {
            //
        }

        public IReadOnlyList<string> GetPackageDirectoriesMarkedForDeletion()
        {
            return new List<string>();
        }
    }
}
