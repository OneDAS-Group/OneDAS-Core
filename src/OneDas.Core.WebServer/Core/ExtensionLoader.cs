using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using NuGet.Common;
using NuGet.Frameworks;
using OneDas.Extensibility;
using OneDas.PackageManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using static NuGet.Frameworks.FrameworkConstants;
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
//                  -: more complex:  skip = DependencyContext.Default.RuntimeLibraries.Any(runtimeLibrary => runtimeLibrary.Name == targetLibrary.Name)
//                  -: it is not possible to update assemblies that the hosting application is referencing by default
//
//      option 4: load everything except "OneDas.Extensibility.Abstractions" into a new load context
//                  +: easy
//                  -: does not work, because then "OneDas.Types" is still loaded twice (throws MethodNotFoundException)
//
//      option 5: load everything except "OneDas.Extensibility.Abstractions" and "OneDas.Types" into a new load context
//                  +: easy
//                  +: all assemblies except "OneDas.Extensibility.Abstractions" and "OneDas.Types" can be updated
//                  -: there is a chance of causing the "MethodNotFoundException" or type mismatches, which needs to be kept in mind
//                  -: dependency injection type resolving prevents usage of option 5
//
// Option 3 is choosed.

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
            _extensionFactory.Clear();

            var assemblySet = this.LoadPackages();

            assemblySet.ToList().ForEach(assembly =>
            {
                try
                {
                    _extensionFactory.ScanAssembly(assembly);
                }
                catch (Exception)
                {
                    //
                }
            });
        }

        private List<Assembly> LoadPackages()
        {
            var assemblySet = new List<Assembly>();
            var loadContext = new OneDasAssemblyLoadContext();

            var lockFile = _packageManager.GetLockFile();
            var lockFileTarget = lockFile?.GetTarget(new NuGetFramework(FrameworkIdentifiers.NetStandard, new Version(2, 1, 0, 0)), Microsoft.DotNet.PlatformAbstractions.RuntimeEnvironment.GetRuntimeIdentifier());

            if (lockFileTarget != null)
            {
                lockFileTarget.Libraries.ToList().ForEach(targetLibrary =>
                {
                    string absoluteFilePath;

                    var lockFileLibrary = lockFile.GetLibrary(targetLibrary.Name, targetLibrary.Version);
                    var basePath = Path.Combine(lockFile.PackageFolders.First().Path, lockFileLibrary.Path);
                    var skip = DependencyContext.Default.RuntimeLibraries.Any(runtimeLibrary => runtimeLibrary.Name == targetLibrary.Name);

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
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            absoluteFilePath = PathUtility.GetPathWithBackSlashes(Path.Combine(basePath, runtimeAssembly.Path));
                        else
                            absoluteFilePath = PathUtility.GetPathWithForwardSlashes(Path.Combine(basePath, runtimeAssembly.Path));

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
