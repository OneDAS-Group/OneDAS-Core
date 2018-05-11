using Microsoft.Extensions.Logging;
using NuGet.Frameworks;
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
        ILogger _logger;
        IExtensionFactory _extensionFactory;
        OneDasPackageManager _packageManager;

        public ExtensionLoader(ILoggerFactory loggerFactory, IExtensionFactory extensionFactory, OneDasPackageManager packageManager)
        {
            _logger = loggerFactory.CreateLogger("Nuget");
            _packageManager = packageManager;
            _extensionFactory = extensionFactory;

            packageManager.InstalledPackagesChanged += (sender, e) => this.ReloadPackages();

            //using (var stream = File.OpenRead("")
            //{
            //    return new DependencyContextJsonReader().Read(stream);
            //}
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

            lockFile = _packageManager.GetLockFile();
            lockFileTarget = lockFile?.GetTarget(FrameworkConstants.CommonFrameworks.NetStandard20, null);
            loadContext = new OneDasAssemblyLoadContext();

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
                        _logger.LogDebug("Loaded lib: " + library.Name);
                    }
                    catch
                    {
                        lockFileLibrary = lockFile.GetLibrary(library.Name, library.Version);
                        basePath = Path.Combine(lockFile.PackageFolders.First().Path, lockFileLibrary.Path);

                        lockFileLibrary.Files.Where(relativeFilePath => relativeFilePath.StartsWith("lib/netstandard2.0") && relativeFilePath.EndsWith(".dll")).ToList().ForEach(relativeFilePath =>
                        {
                            absoluteFilePath = NuGet.Common.PathUtility.GetPathWithBackSlashes(Path.Combine(basePath, relativeFilePath));

                            try
                            {
                                assemblySet.Add(loadContext.LoadFromAssemblyPath(absoluteFilePath));
                                _logger.LogDebug("Loaded file: " + absoluteFilePath);
                            }
                            catch
                            {
                                _logger.LogDebug("Failed to load file: " + absoluteFilePath);
                                throw;
                            }
                        });
                    }
                });
            }

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
