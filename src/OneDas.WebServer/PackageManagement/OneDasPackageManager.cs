using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuGet.Configuration;
using NuGet.PackageManagement;
using NuGet.ProjectManagement;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OneDas.WebServer.PackageManagement
{
    public class OneDasPackageManager
    {
        NuGetPackageManager _nuGetPackageManager;
        SourceRepositoryProvider _sourceRepositoryProvider;
        WebServerOptions _webServerOptions;

        ILoggerFactory _loggerFactory;
        IInstallationCompatibility _installationCompatibility;

        public OneDasPackageManager(IInstallationCompatibility installationCompatibility, IOptions<WebServerOptions> webServerOptions, ILoggerFactory loggerFactory)
        {
            _webServerOptions = webServerOptions.Value;
            _installationCompatibility = installationCompatibility;
            _loggerFactory = loggerFactory;

            _sourceRepositoryProvider = this.CreateSourceRepositoryProvider();
            _nuGetPackageManager = this.CreateNuGetPackageManager(@"O:\dev\Nugetv3 Tests\pack", _sourceRepositoryProvider);
        }

        public async Task<List<IPackageSearchMetadata>> SearchAsync(string searchTerm, string source)
        {
            // aggregate multiple search results:
            // https://github.com/NuGet/NuGet.Client/blob/dev/src/NuGet.Core/NuGet.Indexing/SearchResultsAggregator.cs
            // sourceRepositorySet = new PackageSourceProvider(settings).LoadPackageSources().Select(packageSource => new SourceRepository(packageSource, providerSet)).ToList();

            PackageSource packageSource;
            SourceRepository sourceRepository;
            LoggerAdapter loggerAdapter;
            SearchFilter searchFilter;

            PackageSearchResource packageSearchResource;
            IEnumerable<IPackageSearchMetadata> searchMetadataSet;

            packageSource = new PackageSource(source);
            sourceRepository = _sourceRepositoryProvider.CreateRepository(packageSource);
            loggerAdapter = new LoggerAdapter(new EmptyNuGetProjectContext());
            searchFilter = new SearchFilter(true, null) { PackageTypes = new List<string>() { "Dependency" } }; // _webServerOptions.OneDasName

            packageSearchResource = await sourceRepository.GetResourceAsync<PackageSearchResource>();
            searchMetadataSet = await packageSearchResource.SearchAsync(searchTerm, searchFilter, 0, 100, loggerAdapter, CancellationToken.None);

            return searchMetadataSet.ToList();
        }

        public async Task Install(string packageId, string source)
        {
            PackageSource packageSource;
            SourceRepository sourceRepository;
            ResolutionContext resolutionContext;           
            INuGetProjectContext projectContext;

            resolutionContext = new ResolutionContext(DependencyBehavior.Lowest, includePrelease: true, includeUnlisted: false, VersionConstraints.None);
            projectContext = new EmptyNuGetProjectContext();
            packageSource = new PackageSource(source);
            sourceRepository = _sourceRepositoryProvider.CreateRepository(packageSource);

            await _nuGetPackageManager.InstallPackageAsync(
                        _nuGetPackageManager.PackagesFolderNuGetProject,
                        packageId,
                        resolutionContext,
                        projectContext,
                        new List<SourceRepository>() { sourceRepository }, null, CancellationToken.None);
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
