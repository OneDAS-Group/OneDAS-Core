using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;
using Microsoft.Extensions.Logging;
using NuGet.ProjectModel;
using OneDas.Extensibility;
using OneDas.Extensibility.PackageManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

// https://www.codeproject.com/Articles/1194332/Resolving-Assemblies-in-NET-Core

namespace OneDas.WebServer.Core
{
    public class ExtensionLoader
    {
        private ICompilationAssemblyResolver _assemblyResolver;
        private ILogger _logger;
        private IExtensionFactory _extensionFactory;
        private OneDasPackageManager _packageManager;

        public ExtensionLoader(ILoggerFactory loggerFactory, IExtensionFactory extensionFactory, OneDasPackageManager packageManager)
        {
            _logger = loggerFactory.CreateLogger("Nuget");
            _packageManager = packageManager;
            _extensionFactory = extensionFactory;

            _assemblyResolver = new CompositeCompilationAssemblyResolver
                                (new ICompilationAssemblyResolver[]
            {
                new AppBaseCompilationAssemblyResolver(Directory.GetCurrentDirectory()),
                new ReferenceAssemblyPathResolver(),
                new PackageCompilationAssemblyResolver()
            });

            packageManager.InstalledPackagesChanged += (sender, e) => this.ReloadPackages();
        }

        public void ReloadPackages()
        {
            List<Assembly> assemblySet;

            _extensionFactory.Clear();

            assemblySet = this.LoadPackages();

            assemblySet.ToList().ForEach(assembly =>
            {
                _extensionFactory.ScanAssembly(assembly);
            });
        }

        private List<Assembly> LoadPackages()
        {
            LockFile lockFile;
            LockFileTarget lockFileTarget;
            OneDasAssemblyLoadContext loadContext;
            HashSet<Assembly> assemblySet;

            assemblySet = new HashSet<Assembly>();

            DependencyContext dependencyContext;


            using (FileStream fileStream = File.OpenRead(@"C:\Users\wilvin\AppData\Local\OneDAS\Core\nuget\project.lock.json"))
            {
                dependencyContext = new DependencyContextJsonReader().Read(fileStream);
            }


            var a = dependencyContext.GetDefaultNativeAssets();
            var b = dependencyContext.GetRuntimeNativeAssets("win7-x86");
            var c = dependencyContext.GetRuntimeNativeAssets("win-x86");
            var d = dependencyContext.GetRuntimeNativeAssets("any");
            var e = dependencyContext.GetRuntimeNativeAssets("linux-x64");
            var f = dependencyContext.GetRuntimeNativeAssets("linux-x64");

            
            //dependencyContext.GetDefaultAssemblyNames

            //lockFileTarget = lockFile?.GetTarget(FrameworkConstants.CommonFrameworks.NetStandard20, null);
            //loadContext = new OneDasAssemblyLoadContext();

            //if (lockFileTarget != null)
            //{
            //    lockFileTarget.Libraries.ToList().ForEach(library =>
            //    {
            //        string basePath;
            //        string absoluteFilePath;

            //        LockFileLibrary lockFileLibrary;

            //        try
            //        {
            //            assemblySet.Add(Assembly.Load(library.Name));
            //            _logger.LogDebug("Loaded lib: " + library.Name);
            //        }
            //        catch
            //        {
            //            lockFileLibrary = lockFile.GetLibrary(library.Name, library.Version);
            //            //basePath = Path.Combine(lockFile.PackageFolders.First().Path, lockFileLibrary.Path);

            //            var wrapper = new CompilationLibrary(
            //                lockFileLibrary.Type,
            //                lockFileLibrary.Name,
            //                lockFileLibrary.Version.ToString(),
            //                lockFileLibrary.Sha512,
            //                lockFileLibrary.RuntimeAssemblyGroups.SelectMany(g => g.AssetPaths),
            //                lockFileLibrary.Dependencies,
            //                lockFileLibrary.Serviceable);

            //            var assemblies = new List<string>();

            //            _assemblyResolver.TryResolveAssemblyPaths(, assemblies);

            //            if (assemblies.Count > 0)
            //            {
            //                assemblySet.Add(loadContext.LoadFromAssemblyPath(assemblies[0]));
            //            }

            //            //lockFileLibrary.Files.Where(relativeFilePath => relativeFilePath.StartsWith("lib/netstandard2.0") && relativeFilePath.EndsWith(".dll")).ToList().ForEach(relativeFilePath =>
            //            //{
            //            //    absoluteFilePath = NuGet.Common.PathUtility.GetPathWithBackSlashes(Path.Combine(basePath, relativeFilePath));

            //            //    try
            //            //    {
            //            //        assemblySet.Add(loadContext.LoadFromAssemblyPath(absoluteFilePath));
            //            //        _logger.LogDebug("Loaded file: " + absoluteFilePath);
            //            //    }
            //            //    catch
            //            //    {
            //            //        _logger.LogDebug("Failed to load file: " + absoluteFilePath);
            //            //        throw;
            //            //    }
            //            //});
            //        }
            //    });
            //}

            return assemblySet.ToList();
        }

        private class OneDasAssemblyLoadContext : AssemblyLoadContext
        {
            protected override Assembly Load(AssemblyName assemblyName)
            {
                throw new NotImplementedException();
            }
        }
    }
}
