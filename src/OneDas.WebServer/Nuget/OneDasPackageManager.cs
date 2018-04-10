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
    public class OneDasPackageManager : INuGetProjectContext
    {
        #region "Fields"

        OneDasNugetProject _project;
        NuGetPackageManager _packageManager;
        SourceRepositoryProvider _sourceRepositoryProvider;
        WebServerOptions _webServerOptions;
        LoggerAdapter _loggerAdapter;

        ISettings _settings;
        IInstallationCompatibility _installationCompatibility;

        #endregion

        #region "Constructors"

        public OneDasPackageManager(ISettings settings, IInstallationCompatibility installationCompatibility, IOptions<WebServerOptions> webServerOptions)
        {
            _webServerOptions = webServerOptions.Value;
            _installationCompatibility = installationCompatibility;

            _settings = settings;
            _project = new OneDasNugetProject(_webServerOptions.NugetProjectFilePath);
            _loggerAdapter = new LoggerAdapter(this);
            _sourceRepositoryProvider = this.CreateSourceRepositoryProvider();
            _packageManager = this.CreateNuGetPackageManager(_sourceRepositoryProvider);

            this.PackageExtractionContext = new PackageExtractionContext(PackageSaveMode.Defaultv3, XmlDocFileSaveMode.None, _loggerAdapter, null, null);
            this.PackageSourceSet = SettingsUtility.GetEnabledSources(_settings).ToList();
        }

        #endregion

        #region "Properties"

        public List<PackageSource> PackageSourceSet { get; private set; }

        #endregion

        #region "Methods"

        public List<Assembly> LoadAssemblies()
        {
            List<Assembly> assemblySet;

            assemblySet = new List<Assembly>();

            var lockFile = this.GetLockFile();
            var target = lockFile?.GetTarget(FrameworkConstants.CommonFrameworks.NetStandard20, null);

            if (target != null)
            {
                target.Libraries.ToList().ForEach(library =>
                {
                    try
                    {
                        assemblySet.Add(Assembly.Load(library.Name));
                        Debug.WriteLine("Loaded lib: " + library.Name);
                    }
                    catch
                    {
                        var lockFileLibrary = lockFile.GetLibrary(library.Name, library.Version);
                        var pathBase = Path.Combine(lockFile.PackageFolders.First().Path, lockFileLibrary.Path);

                        lockFileLibrary.Files.Where(relativeFilePath => relativeFilePath.EndsWith(".dll")).ToList().ForEach(relativeFilePath =>
                        {
                            var absoluteFilePath = PathUtility.GetPathWithBackSlashes(Path.Combine(pathBase, relativeFilePath));

                            try
                            {
                                assemblySet.Add(Assembly.LoadFile(absoluteFilePath));
                                Debug.WriteLine("Loaded file: " + absoluteFilePath);
                            }
                            catch
                            {
                                Debug.WriteLine("Failed to load file: " + absoluteFilePath);
                            }
                        });
                    }

                });
            }

            return assemblySet;
        }

        private LockFile GetLockFile()
        {
            return LockFileUtilities.GetLockFile(_project.GetAssetsFilePathAsync().Result, _loggerAdapter);
        }

        public async Task<List<OneDasPackageMetaData>> GetInstalledPackagesAsync()
        {
            var installedPackages = await _project.GetInstalledPackagesAsync(new CancellationToken());

            var packageMetadataSet = installedPackages.
                Where(packageReference => packageReference.IsUserInstalled).
                Select(packageReference => new OneDasPackageMetaData(packageReference.PackageIdentity.Id, string.Empty, packageReference.PackageIdentity.Version.ToString(), true, false)).ToList();

            return packageMetadataSet;
        }

        public async Task<OneDasPackageMetaData[]> SearchAsync(string searchTerm, string source, int skip, int take)
        {
            // aggregate multiple search results:
            // https://github.com/NuGet/NuGet.Client/blob/dev/src/NuGet.Core/NuGet.Indexing/SearchResultsAggregator.cs
            // sourceRepositorySet = new PackageSourceProvider(settings).LoadPackageSources().Select(packageSource => new SourceRepository(packageSource, providerSet)).ToList();
            PackageSource packageSource;
            SourceRepository sourceRepository;
            SearchFilter searchFilter;

            PackageSearchResource packageSearchResource;
            List<Task<OneDasPackageMetaData>> taskSet;
            IEnumerable<IPackageSearchMetadata> searchMetadataSet;

            packageSource = new PackageSource(source);
            sourceRepository = _sourceRepositoryProvider.CreateRepository(packageSource);
            searchFilter = new SearchFilter(true, null) { PackageTypes = new List<string>() { "Dependency" } }; // _webServerOptions.PluginPackageTypeName

            packageSearchResource = await sourceRepository.GetResourceAsync<PackageSearchResource>();
            searchMetadataSet = await packageSearchResource.SearchAsync(searchTerm, searchFilter, skip, take, _loggerAdapter, CancellationToken.None);

            var installedPackages = await _project.GetInstalledPackagesAsync(new CancellationToken());

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
                         this,
                         sourceRepositorySet,
                         null,
                         CancellationToken.None);
        }

        public async Task UpdateAsync(string packageId, string source)
        {
            ResolutionContext resolutionContext;
            PackageDownloadContext packageDownloadContext;
            List<SourceRepository> sourceRepositorySet;

            resolutionContext = new ResolutionContext(DependencyBehavior.Lowest, includePrelease: true, includeUnlisted: false, VersionConstraints.None);
            packageDownloadContext = new PackageDownloadContext(NullSourceCacheContext.Instance);
            sourceRepositorySet = this.PackageSourceSet.Select(packageSource => _sourceRepositoryProvider.CreateRepository(new PackageSource(packageSource.Source))).ToList();

            var actions = await _packageManager.PreviewUpdatePackagesAsync(
                        packageId,
                        new List<NuGetProject>() { _project },
                        resolutionContext,
                        this,
                        sourceRepositorySet,
                        sourceRepositorySet,
                        CancellationToken.None);

            await _packageManager.ExecuteNuGetProjectActionsAsync(_project, actions, this, packageDownloadContext, CancellationToken.None);
        }

        public async Task UninstallAsync(string packageId)
        {
            UninstallationContext uninstallationContext;

            uninstallationContext = new UninstallationContext();

            var installedPackages = await _project.GetInstalledPackagesAsync(new CancellationToken());
            var installedPackage = installedPackages.FirstOrDefault(packageReference => packageReference.PackageIdentity.Id == packageId);

            if (installedPackage != null)
            {
                await _packageManager.UninstallPackageAsync(_project, packageId, uninstallationContext, this, CancellationToken.None);
            }
        }

        private NuGetPackageManager CreateNuGetPackageManager(SourceRepositoryProvider sourceRepositoryProvider)
        {
            NuGetPackageManager packageManager;
            PackageSourceProvider packageSourceProvider;
            OneDasSolutionManager solutionManager;
            OneDasDeleteOnRestartManager deleteOnRestartManager;

            solutionManager = new OneDasSolutionManager(this, _project, _webServerOptions.NugetDirectoryPath);
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

        #endregion

        #region "INugetProjectContext"

        public PackageExtractionContext PackageExtractionContext { get; set; }

        public XDocument OriginalPackagesConfig { get; set; }

        public Guid OperationId { get; set; }

        public NuGetActionType ActionType { get; set; }

        public ISourceControlManagerProvider SourceControlManagerProvider
        {
            get
            {
                return null;
            }
        }

        public NuGet.ProjectManagement.ExecutionContext ExecutionContext
        {
            get
            {
                return null;
            }
        }

        public void Log(MessageLevel level, string message, params object[] args)
        {
            AdvancedBootloader.ClientPushService.SendNugetMessage(string.Format(message, args));
        }

        public void ReportError(string message)
        {
            AdvancedBootloader.ClientPushService.SendNugetMessage(message);
        }

        public FileConflictAction ResolveFileConflict(string message)
        {
            return FileConflictAction.IgnoreAll;
        }

        #endregion
    }
}
