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
        NuGetProject _nuGetProject;
        NuGetPackageManager _nuGetPackageManager;
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
            _nuGetProject = this.CreateNugetProject(@"O:\dev\Nugetv3 Tests\pack", FrameworkConstants.CommonFrameworks.AspNetCore);
            _sourceRepositoryProvider = this.CreateSourceRepositoryProvider();
            _nuGetPackageManager = this.CreateNuGetPackageManager(@"O:\dev\Nugetv3 Tests\pack", _sourceRepositoryProvider);
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

                versionInfoSet = await searchMetadata.GetVersionsAsync();
                isInstalled = versionInfoSet.Any(versionInfo => _nuGetPackageManager.PackageExistsInPackagesFolder(new PackageIdentity(searchMetadata.Identity.Id, new NuGetVersion(versionInfo.Version))));

                isUpdateAvailable = false;

                return new PackageSearchMetadataLight(searchMetadata.Identity.Id, searchMetadata.Description, searchMetadata.Identity.Version.ToString(), isInstalled, isUpdateAvailable);
            }).ToList();

            return await Task.WhenAll(taskSet);
        }

        public async Task InstallAsync(string packageId)
        {
            ResolutionContext resolutionContext;
            List<SourceRepository> sourceRepositorySet;

            resolutionContext = new ResolutionContext(DependencyBehavior.Lowest, includePrelease: true, includeUnlisted: false, VersionConstraints.None);
            sourceRepositorySet = _webServerOptions.PackageSourceSet.Select(packageSource => _sourceRepositoryProvider.CreateRepository(new PackageSource(packageSource.Address))).ToList();

            try
            {
                await _nuGetPackageManager.InstallPackageAsync(
                        _nuGetProject,
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
            List<SourceRepository> sourceRepositorySet;

            uninstallationContext = new UninstallationContext();
            sourceRepositorySet = _webServerOptions.PackageSourceSet.Select(packageSource => _sourceRepositoryProvider.CreateRepository(new PackageSource(packageSource.Address))).ToList();

            var installedPackages = await _nuGetProject.GetInstalledPackagesAsync(new CancellationToken());
            var packageReference = installedPackages.FirstOrDefault(pr => pr.PackageIdentity.Id.Equals(packageId, StringComparison.OrdinalIgnoreCase));

            try
            {
                await _nuGetPackageManager.UninstallPackageAsync(
                        _nuGetProject,
                        packageId,
                        uninstallationContext,
                        _projectContext,
                        CancellationToken.None);
            }
            catch (Exception ex)
            {

                throw;
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

        private NuGetProject CreateNugetProject(string packagesDirectoryPath, NuGetFramework nuGetFramework)
        {
            NuGetProject nuGetProject;
            PackagePathResolver packagePathResolver;

            var dict = new Dictionary<string, object>
                {
                    { NuGetProjectMetadataKeys.Name, "testy" },
                    { NuGetProjectMetadataKeys.TargetFramework, nuGetFramework }
                };

            packagePathResolver = new PackagePathResolver(packagesDirectoryPath);
            nuGetProject = new PackagesConfigNuGetProject(packagesDirectoryPath, dict);
            //nuGetProject = new FolderNuGetProject(packagesDirectoryPath, packagePathResolver, nuGetFramework);

            return nuGetProject;
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
