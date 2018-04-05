using Microsoft.Extensions.Options;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.PackageManagement;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.ProjectManagement;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// https://daveaglick.com/posts/exploring-the-nuget-v3-libraries-part-1

namespace OneDas.WebServer.PackageManagement
{
    public class OneDasPackageManager
    {
        PackagesConfigNuGetProject _project;
        NuGetPackageManager _packageManager;
        SourceRepositoryProvider _sourceRepositoryProvider;
        WebServerOptions _webServerOptions;
        LoggerAdapter _loggerAdapter;

        INuGetProjectContext _projectContext;
        IInstallationCompatibility _installationCompatibility;

        public OneDasPackageManager(IInstallationCompatibility installationCompatibility, IOptions<WebServerOptions> webServerOptions)
        {
            _webServerOptions = webServerOptions.Value;
            _installationCompatibility = installationCompatibility;

            _projectContext = this.CreateNuGetProjectContext();
            _project = this.CreateNugetProject(_webServerOptions.BaseDirectoryPath, FrameworkConstants.CommonFrameworks.AspNetCore);
            _sourceRepositoryProvider = this.CreateSourceRepositoryProvider();
            _packageManager = this.CreateNuGetPackageManager(@"O:\dev\Nugetv3 Tests\pack", _sourceRepositoryProvider);
        }

        public async Task<PackageSearchMetadataLight[]> SearchAsync(string searchTerm, string source)
        {
            // aggregate multiple search results:
            // https://github.com/NuGet/NuGet.Client/blob/dev/src/NuGet.Core/NuGet.Indexing/SearchResultsAggregator.cs
            // sourceRepositorySet = new PackageSourceProvider(settings).LoadPackageSources().Select(packageSource => new SourceRepository(packageSource, providerSet)).ToList();

            PackageSource packageSource;
            SourceRepository sourceRepository;
            SearchFilter searchFilter;

            PackageSearchResource packageSearchResource;
            List<Task<PackageSearchMetadataLight>> taskSet;
            IEnumerable<IPackageSearchMetadata> searchMetadataSet;

            packageSource = new PackageSource(source);
            sourceRepository = _sourceRepositoryProvider.CreateRepository(packageSource);
            searchFilter = new SearchFilter(true, null) { PackageTypes = new List<string>() { "Dependency" } }; // _webServerOptions.PluginPackageTypeName

            packageSearchResource = await sourceRepository.GetResourceAsync<PackageSearchResource>();
            searchMetadataSet = await packageSearchResource.SearchAsync(searchTerm, searchFilter, 0, 15, _loggerAdapter, CancellationToken.None);    

            taskSet = searchMetadataSet.Select(async searchMetadata =>
            {
                bool isInstalled;
                bool isUpdateAvailable;
                IEnumerable<VersionInfo> versionInfoSet;

                isUpdateAvailable = false;
                versionInfoSet = await searchMetadata.GetVersionsAsync();

                var installedPackages = await _project.GetInstalledPackagesAsync(new CancellationToken());
                var installedPackage = installedPackages.FirstOrDefault(packageReference => packageReference.PackageIdentity.Id == searchMetadata.Identity.Id);

                isInstalled = installedPackage != null;

                if (isInstalled)
                {
                    isUpdateAvailable = versionInfoSet.Last().Version.CompareTo(installedPackage.PackageIdentity.Version, VersionComparison.VersionReleaseMetadata) > 0;
                }

                return new PackageSearchMetadataLight(searchMetadata.Identity.Id, searchMetadata.Description, searchMetadata.Identity.Version.ToString(), isInstalled, isUpdateAvailable);
            }).ToList();

            return await Task.WhenAll(taskSet);
        }

        public async Task InstallAsync(string packageId)
        {
            ResolutionContext resolutionContext;
            PackageDownloadContext packageDownloadContext;
            List<SourceRepository> sourceRepositorySet;

            resolutionContext = new ResolutionContext(DependencyBehavior.Lowest, includePrelease: true, includeUnlisted: false, VersionConstraints.None);
            packageDownloadContext = new PackageDownloadContext(NullSourceCacheContext.Instance);
            sourceRepositorySet = _webServerOptions.PackageSourceSet.Select(packageSource => _sourceRepositoryProvider.CreateRepository(new PackageSource(packageSource.Address))).ToList();

            try
            {
                await _packageManager.InstallPackageAsync(
                        _project,
                        packageId,
                        resolutionContext,
                        _projectContext,
                        sourceRepositorySet,
                        null,
                        CancellationToken.None);

                var installedPackages = await _project.GetInstalledPackagesAsync(new CancellationToken());
                //var packageReference = installedPackages.FirstOrDefault(pr => pr.PackageIdentity.Id.Equals(packageId, StringComparison.OrdinalIgnoreCase));

                await Task.WhenAll(installedPackages.ToList().Select(async packageReference => await _packageManager.RestorePackageAsync(
                    packageReference.PackageIdentity, 
                    _projectContext, 
                    packageDownloadContext, 
                    sourceRepositorySet, 
                    CancellationToken.None)));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public async Task UninstallAsync(string packageId)
        {
            UninstallationContext uninstallationContext;
            List<SourceRepository> sourceRepositorySet;

            uninstallationContext = new UninstallationContext();
            sourceRepositorySet = _webServerOptions.PackageSourceSet.Select(packageSource => _sourceRepositoryProvider.CreateRepository(new PackageSource(packageSource.Address))).ToList();

            var installedPackages = await _project.GetInstalledPackagesAsync(new CancellationToken());
            var installedPackage = installedPackages.FirstOrDefault(packageReference => packageReference.PackageIdentity.Id == packageId);

            if (installedPackage != null)
            {
                try
                {
                    await _project.UninstallPackageAsync(
                            installedPackage.PackageIdentity,
                            _projectContext,
                            CancellationToken.None);
                }
                catch (Exception ex)
                {

                    throw;
                }
            }

            
        }

        private INuGetProjectContext CreateNuGetProjectContext()
        {
            EmptyNuGetProjectContext projectContext;

            projectContext = new EmptyNuGetProjectContext();
            _loggerAdapter = new LoggerAdapter(projectContext);
            projectContext.PackageExtractionContext = new PackageExtractionContext(PackageSaveMode.Defaultv3, XmlDocFileSaveMode.None, _loggerAdapter, null);

            return projectContext;
        }

        private PackagesConfigNuGetProject CreateNugetProject(string projectDirectoryPath, NuGetFramework nuGetFramework)
        {
            PackagesConfigNuGetProject project;
            PackagePathResolver packagePathResolver;
            Dictionary<string, object> metadata;

            metadata = new Dictionary<string, object>();
            metadata.Add(NuGetProjectMetadataKeys.Name, _webServerOptions.OneDasName);
            metadata.Add(NuGetProjectMetadataKeys.TargetFramework, nuGetFramework);

            packagePathResolver = new PackagePathResolver(projectDirectoryPath);
            project = new PackagesConfigNuGetProject(projectDirectoryPath, metadata);

            return project;
        }

        private NuGetPackageManager CreateNuGetPackageManager(string packagesDirectoryPath, SourceRepositoryProvider sourceRepositoryProvider)
        {
            NuGetPackageManager packageManager;
            PackageSourceProvider packageSourceProvider;

            packageSourceProvider = (PackageSourceProvider)sourceRepositoryProvider.PackageSourceProvider;
            packageManager = new NuGetPackageManager(sourceRepositoryProvider, packageSourceProvider.Settings, packagesDirectoryPath) { InstallationCompatibility = _installationCompatibility };

            return packageManager;
        }

        private SourceRepositoryProvider CreateSourceRepositoryProvider()
        {
            ISettings settings;
            SourceRepositoryProvider sourceRepositoryProvider;
            List<Lazy<INuGetResourceProvider>> providerSet;

            providerSet = new List<Lazy<INuGetResourceProvider>>();
            providerSet.AddRange(Repository.Provider.GetCoreV3());

            settings = Settings.LoadDefaultSettings(null);

            sourceRepositoryProvider = new SourceRepositoryProvider(settings, providerSet);

            return sourceRepositoryProvider;
        }
    }
}
