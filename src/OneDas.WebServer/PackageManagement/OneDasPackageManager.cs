using Microsoft.Extensions.Options;
using NuGet.Configuration;
using NuGet.PackageManagement;
using NuGet.Packaging;
using NuGet.ProjectManagement;
using NuGet.ProjectManagement.Projects;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// https://daveaglick.com/posts/exploring-the-nuget-v3-libraries-part-1
// https://github.com/NuGet/Home/wiki/NuGet-Restore-No-Op

namespace OneDas.WebServer.PackageManagement
{
    public class OneDasPackageManager
    {
        #region "Fields"

        OneDasNugetProject _oneDasNugetProject;
        NuGetPackageManager _packageManager;
        SourceRepositoryProvider _sourceRepositoryProvider;
        WebServerOptions _webServerOptions;
        LoggerAdapter _loggerAdapter;
        ISettings _settings;

        INuGetProjectContext _projectContext;
        IInstallationCompatibility _installationCompatibility;

        #endregion

        #region "Constructors"

        public OneDasPackageManager(ISettings settings, IInstallationCompatibility installationCompatibility, IOptions<WebServerOptions> webServerOptions)
        {
            _webServerOptions = webServerOptions.Value;
            _installationCompatibility = installationCompatibility;

            _settings = settings;
            _projectContext = this.CreateNuGetProjectContext();
            _oneDasNugetProject = this.CreateNugetProject(_webServerOptions.BaseDirectoryPath);
            _sourceRepositoryProvider = this.CreateSourceRepositoryProvider();
            _packageManager = this.CreateNuGetPackageManager(_sourceRepositoryProvider);

            this.PackageSourceSet = SettingsUtility.GetEnabledSources(_settings).ToList();
        }

        #endregion

        #region "Properties"

        public List<PackageSource> PackageSourceSet { get; private set; }

        #endregion

        #region "Methods"

        public async Task<List<PackageMetaData>> GetInstalledPackagesAsync()
        {
            try
            {
                var installedPackages = await _oneDasNugetProject.GetInstalledPackagesAsync(new CancellationToken());

                var packageMetadataSet = installedPackages.
                    Where(packageReference => packageReference.IsUserInstalled).
                    Select(packageReference => new PackageMetaData(packageReference.PackageIdentity.Id, string.Empty, packageReference.PackageIdentity.Version.ToString(), true, false)).ToList();

                return packageMetadataSet;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<PackageMetaData[]> SearchAsync(string searchTerm, string source)
        {
            // aggregate multiple search results:
            // https://github.com/NuGet/NuGet.Client/blob/dev/src/NuGet.Core/NuGet.Indexing/SearchResultsAggregator.cs
            // sourceRepositorySet = new PackageSourceProvider(settings).LoadPackageSources().Select(packageSource => new SourceRepository(packageSource, providerSet)).ToList();
            PackageSource packageSource;
            SourceRepository sourceRepository;
            SearchFilter searchFilter;

            PackageSearchResource packageSearchResource;
            List<Task<PackageMetaData>> taskSet;
            IEnumerable<IPackageSearchMetadata> searchMetadataSet;

            packageSource = new PackageSource(source);
            sourceRepository = _sourceRepositoryProvider.CreateRepository(packageSource);
            searchFilter = new SearchFilter(true, null) { PackageTypes = new List<string>() { "Dependency" } }; // _webServerOptions.PluginPackageTypeName

            packageSearchResource = await sourceRepository.GetResourceAsync<PackageSearchResource>();
            searchMetadataSet = await packageSearchResource.SearchAsync(searchTerm, searchFilter, 0, 15, _loggerAdapter, CancellationToken.None);

            var installedPackages = await _oneDasNugetProject.GetInstalledPackagesAsync(new CancellationToken());

            taskSet = searchMetadataSet.Select(async searchMetadata =>
            {
                bool isInstalled;
                bool isUpdateAvailable;
                IEnumerable<VersionInfo> versionInfoSet;

                isUpdateAvailable = false;
                versionInfoSet = await searchMetadata.GetVersionsAsync();

                var installedPackage = installedPackages.FirstOrDefault(packageReference => packageReference.PackageIdentity.Id == searchMetadata.Identity.Id);

                isInstalled = installedPackage != null;

                if (isInstalled)
                {
                    isUpdateAvailable = versionInfoSet.Last().Version.CompareTo(installedPackage.PackageIdentity.Version, VersionComparison.VersionReleaseMetadata) > 0;
                }

                return new PackageMetaData(searchMetadata.Identity.Id, searchMetadata.Description, searchMetadata.Identity.Version.ToString(), isInstalled, isUpdateAvailable);
            }).ToList();

            return await Task.WhenAll(taskSet);
        }

        public async Task InstallAsync(string packageId, string source)
        {
            ResolutionContext resolutionContext;
            PackageDownloadContext packageDownloadContext;
            List<SourceRepository> sourceRepositorySet;

            resolutionContext = new ResolutionContext(DependencyBehavior.Lowest, includePrelease: true, includeUnlisted: false, VersionConstraints.None);
            packageDownloadContext = new PackageDownloadContext(NullSourceCacheContext.Instance);
            sourceRepositorySet = this.PackageSourceSet.Select(packageSource => _sourceRepositoryProvider.CreateRepository(new PackageSource(packageSource.Source))).ToList();

            try
            {
                await _packageManager.InstallPackageAsync(
                        _oneDasNugetProject,
                        packageId,
                        resolutionContext,
                        _projectContext,
                        sourceRepositorySet,
                        null,
                        CancellationToken.None);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public async Task UninstallAsync(string packageId)
        {
            UninstallationContext uninstallationContext;

            uninstallationContext = new UninstallationContext();

            var installedPackages = await _oneDasNugetProject.GetInstalledPackagesAsync(new CancellationToken());
            var installedPackage = installedPackages.FirstOrDefault(packageReference => packageReference.PackageIdentity.Id == packageId);

            if (installedPackage != null)
            {
                try
                {
                    await _oneDasNugetProject.UninstallPackageAsync(
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
            EmptyNuGetProjectContext2 projectContext;

            projectContext = new EmptyNuGetProjectContext2();
            _loggerAdapter = new LoggerAdapter(projectContext);

            projectContext.PackageExtractionContext = new PackageExtractionContext(PackageSaveMode.Defaultv3, XmlDocFileSaveMode.None, _loggerAdapter, null);

            return projectContext;
        }

        private OneDasNugetProject CreateNugetProject(string projectDirectoryPath)
        {
            OneDasNugetProject project;

            project = new OneDasNugetProject(Path.Combine(projectDirectoryPath, "project.json"));

            return project;
        }

        private NuGetPackageManager CreateNuGetPackageManager(SourceRepositoryProvider sourceRepositoryProvider)
        {
            NuGetPackageManager packageManager;
            PackageSourceProvider packageSourceProvider;
            OneDasSolutionManager solutionManager;
            OneDasDeleteOnRestartManager deleteOnRestartManager;

            solutionManager = new OneDasSolutionManager(_projectContext, _oneDasNugetProject, _webServerOptions.BaseDirectoryPath);
            deleteOnRestartManager = new OneDasDeleteOnRestartManager();
            packageSourceProvider = (PackageSourceProvider)sourceRepositoryProvider.PackageSourceProvider;
            packageManager = new NuGetPackageManager(sourceRepositoryProvider, _settings, solutionManager, deleteOnRestartManager) { InstallationCompatibility = _installationCompatibility };

            return packageManager;
        }

        private SourceRepositoryProvider CreateSourceRepositoryProvider()
        {
            SourceRepositoryProvider sourceRepositoryProvider;
            List<Lazy<INuGetResourceProvider>> providerSet;

            providerSet = new List<Lazy<INuGetResourceProvider>>();
            providerSet.AddRange(Repository.Provider.GetCoreV3());

            sourceRepositoryProvider = new SourceRepositoryProvider(_settings, providerSet);

            return sourceRepositoryProvider;
        }

        #endregion
    }
}
