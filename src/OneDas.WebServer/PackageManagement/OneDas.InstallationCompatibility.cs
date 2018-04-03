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

namespace OneDas.WebServer.PackageManagement
{
    public class OneDasInstallationCompatibility : IInstallationCompatibility
    {
        WebServerOptions _webServerOptions;

        public OneDasInstallationCompatibility(IOptions<WebServerOptions> webServerOptions)
        {
            _webServerOptions = webServerOptions.Value;
        }

        public void EnsurePackageCompatibility(NuGetProject nuGetProject, INuGetPathContext pathContext, IEnumerable<NuGetProjectAction> nuGetProjectActions, RestoreResult restoreResult)
        {
            string manifestPath;

            NuspecReader nuspecReader;

            FallbackPackagePathResolver resolver;
            FallbackPackagePathInfo packageInfo;

            HashSet<string> requestedPackageIds;
            IEnumerable<PackageIdentity> installedIdentities;

            // Find all of the installed package identities.
            requestedPackageIds = new HashSet<string>(nuGetProjectActions.Where(action => action.NuGetProjectActionType == NuGetProjectActionType.Install).Select(action => action.PackageIdentity.Id), StringComparer.OrdinalIgnoreCase);

            installedIdentities = restoreResult
                .RestoreGraphs
                .SelectMany(graph => graph.Flattened)
                .Where(result => result.Key.Type == LibraryType.Package)
                .Select(result => new PackageIdentity(result.Key.Name, result.Key.Version))
                .Distinct()
                .Where(identity => requestedPackageIds.Contains(identity.Id));

            // Read the .nuspec on disk and ensure package compatibility.
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
            MinClientVersionUtility.VerifyMinClientVersion(nuspecReader);

            if (!nuspecReader.GetPackageTypes().Any(packageType => packageType.Name == _webServerOptions.OneDasName))
            {
                throw new Exception(ErrorMessage.InstallationCompatiblity_InvalidPackageType);
            }
        }
    }
}