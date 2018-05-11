using Microsoft.Extensions.Options;
using NuGet.Commands;
using NuGet.Common;
using NuGet.LibraryModel;
using NuGet.PackageManagement;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.ProjectManagement;
using NuGet.Protocol.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OneDas.Extensibility.PackageManagement
{
    public class OneDasInstallationCompatibility : IInstallationCompatibility
    {
        #region "Fields"

        OneDasOptions _options;

        #endregion

        #region "Constructors"

        public OneDasInstallationCompatibility(IOptions<OneDasOptions> options)
        {
            _options = options.Value;
        }

        #endregion

        #region "Methods"

        public void EnsurePackageCompatibility(NuGetProject nuGetProject, INuGetPathContext pathContext, IEnumerable<NuGetProjectAction> nuGetProjectActions, RestoreResult restoreResult)
        {
            string manifestPath;

            NuspecReader nuspecReader;

            FallbackPackagePathResolver resolver;
            FallbackPackagePathInfo packageInfo;

            HashSet<string> requestedPackageIds;
            IEnumerable<PackageIdentity> installedIdentities;

            // find all of the installed package identities.
            requestedPackageIds = new HashSet<string>(nuGetProjectActions.Where(action => action.NuGetProjectActionType == NuGetProjectActionType.Install).Select(action => action.PackageIdentity.Id), StringComparer.OrdinalIgnoreCase);

            installedIdentities = restoreResult
                .RestoreGraphs
                .SelectMany(graph => graph.Flattened)
                .Where(result => result.Key.Type == LibraryType.Package)
                .Select(result => new PackageIdentity(result.Key.Name, result.Key.Version))
                .Distinct()
                .Where(identity => requestedPackageIds.Contains(identity.Id));

            // read the .nuspec on disk and ensure package compatibility.
            resolver = new FallbackPackagePathResolver(pathContext);

            foreach (PackageIdentity identity in installedIdentities)
            {
                packageInfo = resolver.GetPackageInfo(identity.Id, identity.Version);

                if (packageInfo != null)
                {
                    manifestPath = packageInfo.PathResolver.GetManifestFilePath(identity.Id, identity.Version);
                    nuspecReader = new NuspecReader(manifestPath);

                    this.EnsurePackageCompatibility(nuGetProject, identity, nuspecReader);
                }
            }
        }

        public async Task EnsurePackageCompatibilityAsync(NuGetProject nuGetProject, PackageIdentity packageIdentity, DownloadResourceResult resourceResult, CancellationToken cancellationToken)
        {
            NuspecReader nuspecReader;

            if (nuGetProject == null)
            {
                throw new ArgumentNullException(nameof(nuGetProject));
            }

            if (packageIdentity == null)
            {
                throw new ArgumentNullException(nameof(packageIdentity));
            }

            if (resourceResult == null)
            {
                throw new ArgumentNullException(nameof(resourceResult));
            }

            cancellationToken.ThrowIfCancellationRequested();

            if (resourceResult.PackageReader != null)
            {
                nuspecReader = await resourceResult.PackageReader.GetNuspecReaderAsync(cancellationToken);
            }
            else
            {
                using (PackageArchiveReader packageReader = new PackageArchiveReader(resourceResult.PackageStream, leaveStreamOpen: true))
                {
                    nuspecReader = packageReader.NuspecReader;
                }
            }

            this.EnsurePackageCompatibility(nuGetProject, packageIdentity, nuspecReader);
        }

        private void EnsurePackageCompatibility(NuGetProject nuGetProject, PackageIdentity packageIdentity, NuspecReader nuspecReader)
        {
            IReadOnlyList<PackageType> packageTypeSet;

            MinClientVersionUtility.VerifyMinClientVersion(nuspecReader);

            packageTypeSet = nuspecReader.GetPackageTypes();

            if (packageTypeSet.Any())
            {
                if (!packageTypeSet.Any(packageType => packageType == PackageType.Dependency || packageType.Name == _options.PackageTypeName))
                {
                    throw new Exception(ErrorMessage.OneDasInstallationCompatiblity_InvalidPackageType);
                }
            }
        }

        #endregion
    }
}