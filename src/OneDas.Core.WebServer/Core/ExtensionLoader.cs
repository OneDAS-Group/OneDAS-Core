using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.Logging;
using NuGet.Common;
using NuGet.Frameworks;
using NuGet.ProjectModel;
using OneDas.Extensibility;
using OneDas.Extensibility.PackageManagement;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using ILogger = Microsoft.Extensions.Logging.ILogger;

// https://www.codeproject.com/Articles/1194332/Resolving-Assemblies-in-NET-Core

// Working principle:
// 
//      option 1: load all assemblies into the default load context
//                  +: easy
//                  -: unable to load new assembly versions
//
//      option 2: load all assemblies into a new load context
//                  +: easy
//                  -: does not work, because "OneDas.Extensibility.Abstractions" is loaded twice and therefore inheritance does not work properly
//
//      option 3: load only new assemblies into a new load context
//                  -: more complex:  if (DependencyContext.Default.RuntimeLibraries.Any(runtimeLibrary => runtimeLibrary.Name == targetLibrary.Name && runtimeLibrary.Version == targetLibrary.Version.ToString())) ...
//                  -: it is not possible to update assemblies that the hosting application is referencing by default
//
//      option 4: load everything except "OneDas.Extensibility.Abstractions" into a new load context
//                  +: easy
//                  -: does not work, because then "OneDas.Types" is still loaded twice (throws MethodNotFoundException)
//
//      option 5: load everything except "OneDas.Extensibility.Abstractions" and "OneDas.Types" into a new load context
//                  +: easy
//                  +: all assemblies except "OneDas.Extensibility.Abstractions" and "OneDas.Types" can be updated
//                  -: there is a small chance of causing the "MethodNotFoundException", which needs to be kept in mind
//
// Option 5 is choosed.

namespace OneDas.WebServer.Core
{
    public class ExtensionLoader
    {
        private ILogger _logger;
        private IExtensionFactory _extensionFactory;
        private OneDasPackageManager _packageManager;

        public ExtensionLoader(ILoggerFactory loggerFactory, IExtensionFactory extensionFactory, OneDasPackageManager packageManager)
        {
            _logger = loggerFactory.CreateLogger("ExtensionLoader");
            _packageManager = packageManager;
            _extensionFactory = extensionFactory;

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
            List<Assembly> assemblySet;

            assemblySet = new List<Assembly>();
            loadContext = new OneDasAssemblyLoadContext();

            lockFile = _packageManager.GetLockFile();
            lockFileTarget = lockFile?.GetTarget(FrameworkConstants.CommonFrameworks.NetStandard20, RuntimeEnvironment.GetRuntimeIdentifier());

            if (lockFileTarget != null)
            {
                lockFileTarget.Libraries.ToList().ForEach(targetLibrary =>
                {
                    bool skip;
                    string basePath;
                    string absoluteFilePath;

                    LockFileLibrary lockFileLibrary;

                    lockFileLibrary = lockFile.GetLibrary(targetLibrary.Name, targetLibrary.Version);
                    basePath = Path.Combine(lockFile.PackageFolders.First().Path, lockFileLibrary.Path);
                    skip = targetLibrary.Name == "OneDas.Extensibility.Abstractions" || targetLibrary.Name == "OneDas.Types";

                    if (skip)
                    {
                        _logger.LogDebug($"#### skipping library: { targetLibrary.Name }/{ targetLibrary.Version.ToString() }");
                        return;
                    }
                    else
                    {
                        _logger.LogDebug($"#### processing library: { targetLibrary.Name }");
                    }

                    // RuntimeAssemblies
                    targetLibrary.RuntimeAssemblies.ToList().ForEach(runtimeAssembly =>
                    {
                        absoluteFilePath = PathUtility.GetPathWithBackSlashes(Path.Combine(basePath, runtimeAssembly.Path));

                        if (!runtimeAssembly.Path.EndsWith("/_._"))
                        {
                            _logger.LogDebug($"processing file: { absoluteFilePath }");
                        }
                        else
                        {
                            _logger.LogDebug($"skipping file: { absoluteFilePath }");
                            return;
                        }

                        try
                        {
                            assemblySet.Add(loadContext.LoadFromAssemblyPath(absoluteFilePath));
                            _logger.LogDebug($"file loaded: { absoluteFilePath }");
                        }
                        catch
                        {
                            _logger.LogDebug($"file loading failed: { absoluteFilePath }");
                            throw;
                        }
                    });

                    // NativeLibraries
                    targetLibrary.NativeLibraries.ToList().ForEach(nativeLibrary =>
                    {
                        absoluteFilePath = PathUtility.GetPathWithBackSlashes(Path.Combine(basePath, nativeLibrary.Path));

                        _logger.LogDebug($"processing file: { absoluteFilePath }");

                        try
                        {
                            loadContext.LoadFromNativeAssemblyPath(absoluteFilePath);
                            _logger.LogDebug($"native file loaded: { absoluteFilePath }");
                        }
                        catch
                        {
                            _logger.LogDebug($"native file loading failed: { absoluteFilePath }");
                            throw;
                        }
                    });
                });
            }

            return assemblySet.ToList();
        }

        private class OneDasAssemblyLoadContext : AssemblyLoadContext
        {
            public void LoadFromNativeAssemblyPath(string assemblyFilePath)
            {
                this.LoadUnmanagedDllFromPath(assemblyFilePath);
            }

            protected override Assembly Load(AssemblyName assemblyName)
            {
                return null;
            }
        }
    }
}
