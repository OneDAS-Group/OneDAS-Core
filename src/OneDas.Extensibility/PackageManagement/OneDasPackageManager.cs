using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OneDas.Extensibility.PackageManagement
{
    public class OneDasPackageManager
    {
        #region "Events"

        public event EventHandler InstalledPackagesChanged;

        #endregion

        #region "Fields"

        OneDasOptions _options;
        OneDasNugetProject _project;
        NuGetPackageManager _packageManager;
        SourceRepositoryProvider _sourceRepositoryProvider;
        OneDasNuGetProjectContext _projectContext;

        ISettings _settings;
        IExtensionFactory _extensionFactory;

        #endregion

        #region "Constructors"

        public OneDasPackageManager(IExtensionFactory extensionFactory, IOptions<OneDasOptions> options, ILoggerFactory loggerFactory)
        {
            JObject jobject;
            VersionRange versionRange;

            _extensionFactory = extensionFactory;
            _options = options.Value;

            // settings
            _settings = new Settings(_options.NugetDirectoryPath);
            _settings.SetValues(ConfigurationConstants.PackageSources, new List<SettingValue>() { new SettingValue("MyGet (CI)", "https://www.myget.org/F/onedas/api/v3/index.json", false) });

            if (!File.Exists(_options.NugetProjectFilePath))
            {
                jobject = new JObject();
                versionRange = new VersionRange(new NuGetVersion("2.0.3"));

                JsonConfigUtility.AddFramework(jobject, FrameworkConstants.CommonFrameworks.NetStandard20);
                JsonConfigUtility.AddDependency(jobject, new PackageDependency("NETStandard.Library", versionRange));

                jobject.Add("runtimes", new JObject(new JProperty(_options.RestoreRuntimeId, new JObject())));

                File.WriteAllText(_options.NugetProjectFilePath, jobject.ToString(Formatting.Indented));
            }

            _project = new OneDasNugetProject(_options.NugetProjectFilePath);
            _projectContext = new OneDasNuGetProjectContext(loggerFactory.CreateLogger("Nuget"));
            _sourceRepositoryProvider = this.CreateSourceRepositoryProvider();
            _packageManager = this.CreateNuGetPackageManager(_sourceRepositoryProvider);

            this.PackageSourceSet = SettingsUtility.GetEnabledSources(_settings).ToList();
        }

        #endregion

        #region "Properties"

        public List<PackageSource> PackageSourceSet { get; private set; }

        #endregion

        #region "Methods"

        public LockFile GetLockFile()
        {
            return LockFileUtilities.GetLockFile(_project.GetAssetsFilePathAsync().Result, _projectContext.LoggerAdapter);
        }

        public async Task<List<OneDasPackageMetaData>> GetInstalledPackagesAsync()
        {
            IEnumerable<PackageReference> installedPackageSet;
            List<OneDasPackageMetaData> packageMetadataSet;

            installedPackageSet = await _project.GetInstalledPackagesAsync(new CancellationToken());

            packageMetadataSet = installedPackageSet.
                    Where(packageReference => packageReference.PackageIdentity.Id != "NETStandard.Library").
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
            searchFilter = new SearchFilter(true) { PackageTypes = new List<string>() { "Dependency" } }; // _options.PackageTypeName -> not really working

            packageSearchResource = await sourceRepository.GetResourceAsync<PackageSearchResource>();
            searchMetadataSet = await packageSearchResource.SearchAsync($"{ searchTerm.Replace(" ", "+") }+{ _options.PackageTypeName }", searchFilter, skip, take, _projectContext.LoggerAdapter, CancellationToken.None);

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

            this.OnInstalledPackagesChanged();
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

            this.OnInstalledPackagesChanged();
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

            this.OnInstalledPackagesChanged();
        }

        private void OnInstalledPackagesChanged()
        {
            this.InstalledPackagesChanged?.Invoke(this, EventArgs.Empty);
        }

        private NuGetPackageManager CreateNuGetPackageManager(SourceRepositoryProvider sourceRepositoryProvider)
        {
            NuGetPackageManager packageManager;
            PackageSourceProvider packageSourceProvider;
            OneDasSolutionManager solutionManager;
            OneDasDeleteOnRestartManager deleteOnRestartManager;

            solutionManager = new OneDasSolutionManager(_projectContext, _project, _options.NugetDirectoryPath);
            deleteOnRestartManager = new OneDasDeleteOnRestartManager();
            packageSourceProvider = (PackageSourceProvider)sourceRepositoryProvider.PackageSourceProvider;
            packageManager = new NuGetPackageManager(sourceRepositoryProvider, _settings, solutionManager, deleteOnRestartManager);

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