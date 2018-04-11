using Microsoft.Extensions.Options;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.PackageManagement;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.ProjectManagement;
using NuGet.ProjectModel;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using NuGet.Versioning;
using OneDas.Plugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OneDas.WebServer.Nuget
{
    public class OneDasPackageManager
    {
        #region "Fields"

        WebServerOptions _webServerOptions;
        OneDasNugetProject _project;
        NuGetPackageManager _packageManager;
        SourceRepositoryProvider _sourceRepositoryProvider;
        OneDasNuGetProjectContext _projectContext;

        ISettings _settings;
        IPluginProvider _pluginProvider;
        IInstallationCompatibility _installationCompatibility;

        #endregion

        #region "Constructors"

        public OneDasPackageManager(IPluginProvider pluginProvider, ISettings settings, IInstallationCompatibility installationCompatibility, IOptions<WebServerOptions> webServerOptions)
        {
            _pluginProvider = pluginProvider;
            _settings = settings;
            _installationCompatibility = installationCompatibility;
            _webServerOptions = webServerOptions.Value;

            _project = new OneDasNugetProject(_webServerOptions.NugetProjectFilePath);
            _projectContext = new OneDasNuGetProjectContext();
            _sourceRepositoryProvider = this.CreateSourceRepositoryProvider();
            _packageManager = this.CreateNuGetPackageManager(_sourceRepositoryProvider);

            this.PackageSourceSet = SettingsUtility.GetEnabledSources(_settings).ToList();
        }

        #endregion

        #region "Properties"

        public List<PackageSource> PackageSourceSet { get; private set; }

        #endregion

        #region "Methods"

        public Task ReloadPackagesAsync()
        {
            return Task.Run(() =>
            {
                this.ReloadPackages();
            });
        }

        public void ReloadPackages()
        {
            List<Assembly> assemblySet;

            _pluginProvider.Clear();

            assemblySet = this.LoadPackages();

            assemblySet.ToList().ForEach(assembly =>
            {
                _pluginProvider.ScanAssembly(assembly);
            });
        }

        public async Task<List<OneDasPackageMetaData>> GetInstalledPackagesAsync()
        {
            IEnumerable<PackageReference> installedPackageSet;
            List<OneDasPackageMetaData> packageMetadataSet;

            installedPackageSet = await _project.GetInstalledPackagesAsync(new CancellationToken());

            packageMetadataSet = installedPackageSet.
                    Where(packageReference => packageReference.IsUserInstalled).
                    Select(packageReference => new OneDasPackageMetaData(packageReference.PackageIdentity.Id, string.Empty, packageReference.PackageIdentity.Version.ToString(), true, false)).ToList();

            return packageMetadataSet;
        }

        public async Task<OneDasPackageMetaData[]> SearchAsync(string searchTerm, string source, int skip, int take)
        {
            PackageSource packageSource;
            SourceRepository sourceRepository;
            SearchFilter searchFilter;

            PackageSearchResource packageSearchResource;
            List<Task<OneDasPackageMetaData>> taskSet;
            IEnumerable<PackageReference> installedPackageSet;
            IEnumerable<IPackageSearchMetadata> searchMetadataSet;

            packageSource = new PackageSource(source);
            sourceRepository = _sourceRepositoryProvider.CreateRepository(packageSource);
            searchFilter = new SearchFilter(true, null) { PackageTypes = new List<string>() { "Dependency" } }; // _webServerOptions.PluginPackageTypeName

            packageSearchResource = await sourceRepository.GetResourceAsync<PackageSearchResource>();
            searchMetadataSet = await packageSearchResource.SearchAsync(searchTerm, searchFilter, skip, take, _projectContext.LoggerAdapter, CancellationToken.None);

            installedPackageSet = await _project.GetInstalledPackagesAsync(new CancellationToken());

            taskSet = searchMetadataSet.Select(async searchMetadata =>
            {
                bool isInstalled;
                bool isUpdateAvailable;
                PackageReference installedPackage;
                IEnumerable<VersionInfo> versionInfoSet;

                isUpdateAvailable = false;
                versionInfoSet = await searchMetadata.GetVersionsAsync();
                installedPackage = installedPackageSet.FirstOrDefault(packageReference => packageReference.PackageIdentity.Id == searchMetadata.Identity.Id);
                isInstalled = installedPackage != null;

                if (isInstalled)
                {
                    isUpdateAvailable = versionInfoSet.Last().Version.CompareTo(installedPackage.PackageIdentity.Version, VersionComparison.VersionReleaseMetadata) > 0;
                }

                return new OneDasPackageMetaData(searchMetadata.Identity.Id, searchMetadata.Description, searchMetadata.Identity.Version.ToString(), isInstalled, isUpdateAvailable);
            }).ToList();

            return await Task.WhenAll(taskSet);
        }

        public async Task InstallAsync(string packageId, string source)
        {
            ResolutionContext resolutionContext;
            List<SourceRepository> sourceRepositorySet;

            resolutionContext = new ResolutionContext(DependencyBehavior.Lowest, includePrelease: true, includeUnlisted: false, VersionConstraints.None);
            sourceRepositorySet = this.PackageSourceSet.Select(packageSource => _sourceRepositoryProvider.CreateRepository(new PackageSource(packageSource.Source))).ToList();

            await _packageManager.InstallPackageAsync(
                         _project,
                         packageId,
                         resolutionContext,
                         _projectContext,
                         sourceRepositorySet,
                         null,
                         CancellationToken.None);
        }

        public async Task UpdateAsync(string packageId, string source)
        {
            ResolutionContext resolutionContext;
            PackageDownloadContext packageDownloadContext;
            List<SourceRepository> sourceRepositorySet;
            IEnumerable<NuGetProjectAction> actionSet;

            resolutionContext = new ResolutionContext(DependencyBehavior.Lowest, includePrelease: true, includeUnlisted: false, VersionConstraints.None);
            packageDownloadContext = new PackageDownloadContext(NullSourceCacheContext.Instance);
            sourceRepositorySet = this.PackageSourceSet.Select(packageSource => _sourceRepositoryProvider.CreateRepository(new PackageSource(packageSource.Source))).ToList();

            actionSet = await _packageManager.PreviewUpdatePackagesAsync(
                        packageId,
                        new List<NuGetProject>() { _project },
                        resolutionContext,
                        _projectContext,
                        sourceRepositorySet,
                        sourceRepositorySet,
                        CancellationToken.None);

            await _packageManager.ExecuteNuGetProjectActionsAsync(_project, actionSet, _projectContext, packageDownloadContext, CancellationToken.None);
        }

        public async Task UninstallAsync(string packageId)
        {
            UninstallationContext uninstallationContext;
            PackageReference installedPackage;
            IEnumerable<PackageReference> installedPackageSet;

            uninstallationContext = new UninstallationContext();

            installedPackageSet = await _project.GetInstalledPackagesAsync(new CancellationToken());
            installedPackage = installedPackageSet.FirstOrDefault(packageReference => packageReference.PackageIdentity.Id == packageId);

            if (installedPackage != null)
            {
                await _packageManager.UninstallPackageAsync(_project, packageId, uninstallationContext, _projectContext, CancellationToken.None);
            }
        }

        private NuGetPackageManager CreateNuGetPackageManager(SourceRepositoryProvider sourceRepositoryProvider)
        {
            NuGetPackageManager packageManager;
            PackageSourceProvider packageSourceProvider;
            OneDasSolutionManager solutionManager;
            OneDasDeleteOnRestartManager deleteOnRestartManager;

            solutionManager = new OneDasSolutionManager(_projectContext, _project, _webServerOptions.NugetDirectoryPath);
            deleteOnRestartManager = new OneDasDeleteOnRestartManager();
            packageSourceProvider = (PackageSourceProvider)sourceRepositoryProvider.PackageSourceProvider;

            packageManager = new NuGetPackageManager(sourceRepositoryProvider, _settings, solutionManager, deleteOnRestartManager)
            {
                InstallationCompatibility = _installationCompatibility
            };

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

        private List<Assembly> LoadPackages()
        {
            LockFile lockFile;
            LockFileTarget lockFileTarget;
            HashSet<Assembly> assemblySet;

            assemblySet = new HashSet<Assembly>();

            lockFile = this.GetLockFile();
            lockFileTarget = lockFile?.GetTarget(FrameworkConstants.CommonFrameworks.NetStandard20, null);

            if (lockFileTarget != null)
            {
                lockFileTarget.Libraries.ToList().ForEach(library =>
                {
                    string basePath;
                    string absoluteFilePath;

                    LockFileLibrary lockFileLibrary;

                    try
                    {
                        assemblySet.Add(Assembly.Load(library.Name));
                        _projectContext.Log(MessageLevel.Debug, "Loaded lib: " + library.Name);
                    }
                    catch
                    {
                        lockFileLibrary = lockFile.GetLibrary(library.Name, library.Version);
                        basePath = Path.Combine(lockFile.PackageFolders.First().Path, lockFileLibrary.Path);

                        lockFileLibrary.Files.Where(relativeFilePath => relativeFilePath.StartsWith("lib/netstandard2.0") && relativeFilePath.EndsWith(".dll")).ToList().ForEach(relativeFilePath =>
                        {
                            absoluteFilePath = PathUtility.GetPathWithBackSlashes(Path.Combine(basePath, relativeFilePath));

                            try
                            {
                                assemblySet.Add(Assembly.LoadFrom(absoluteFilePath));
                                _projectContext.Log(MessageLevel.Debug, "Loaded file: " + absoluteFilePath);
                            }
                            catch
                            {
                                _projectContext.Log(MessageLevel.Debug, "Failed to load file: " + absoluteFilePath);
                                throw;
                            }
                        });
                    }
                });
            }

            return assemblySet.ToList();
        }

        private LockFile GetLockFile()
        {
            return LockFileUtilities.GetLockFile(_project.GetAssetsFilePathAsync().Result, _projectContext.LoggerAdapter);
        }

        #endregion
    }
}
