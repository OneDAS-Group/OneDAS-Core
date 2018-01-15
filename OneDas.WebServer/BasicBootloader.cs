using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using OneDas.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;

namespace OneDas.WebServer
{
    public static class BasicBootloader
    {
        #region "Fields"

        private static WebServerOptions _webServerOptions;
        private static ServiceController _oneDasServiceController;
        private static AdvancedBootloader _advancedBootloader;

        #endregion

        #region "Methods"

        public static void Main(string[] args)
        {
            bool isHosting;
            string configurationDirectoryPath;
            string configurationFileName;
            IConfigurationRoot configurationRoot;
            IConfigurationBuilder configurationBuilder;

            // configuration
            configurationDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OneDAS");
            configurationFileName = "onedassettings.json";

            Directory.CreateDirectory(configurationDirectoryPath);

            if (!File.Exists(Path.Combine(configurationDirectoryPath, configurationFileName)))
            {
                File.WriteAllText(Path.Combine(configurationDirectoryPath, configurationFileName), "{ Dummy: 0 }");
            }

            configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile(new PhysicalFileProvider(configurationDirectoryPath), path: configurationFileName, optional: false, reloadOnChange: true);

            configurationRoot = configurationBuilder.Build();
            _webServerOptions = configurationRoot.Get<WebServerOptions>();

            // base directory path
            if (!Directory.Exists(_webServerOptions.BaseDirectoryPath))
            {
                _webServerOptions.BaseDirectoryPath = configurationDirectoryPath;
            }

            try
            {
                if (!string.Equals(Path.GetFullPath(_webServerOptions.BaseDirectoryPath), Path.GetFullPath(_webServerOptions.NewBaseDirectoryPath), StringComparison.OrdinalIgnoreCase))
                {
                    _webServerOptions.BaseDirectoryPath = _webServerOptions.NewBaseDirectoryPath;
                }
            }
            catch
            {
                _webServerOptions.NewBaseDirectoryPath = _webServerOptions.BaseDirectoryPath;
            }
            finally
            {
                _webServerOptions.Save(_webServerOptions.BaseDirectoryPath);
            }

            // parse command line
            BasicBootloader.ParseCommandLineArgumentSet(args);

            // ensure AppDomain shadow copying
            BasicBootloader.EnsureAppDomainShadowCopying();

            // OneDasServiceController
            _oneDasServiceController = ServiceController.GetServices().FirstOrDefault(serviceController =>
            {
                return serviceController.ServiceName == _webServerOptions.ServiceName;
            });

            // create advances boot loader
            isHosting = BasicBootloader.GetOneDasServiceStatus() == 0;
            _advancedBootloader = new AdvancedBootloader(isHosting, _webServerOptions, configurationRoot);

            // logging
            BasicBootloader.SystemLogger = _advancedBootloader.CreateSystemLogger();

            // exception handling
            AppDomain.CurrentDomain.UnhandledException += BasicBootloader.CurrentDomain_UnhandledException;

            // run
            _advancedBootloader.Run();
        }

        private static void ParseCommandLineArgumentSet(IList<string> commandLineArgumentSet)
        {
            Contract.Requires(commandLineArgumentSet != null);

            if (!commandLineArgumentSet.Contains("-ALLOWSINGELTON"))
            {
                if (!(Environment.UserInteractive && BasicBootloader.GetOneDasServiceStatus() > 0))
                {
                    if (!SingeltonHelper.EnsureSingeltonInstance(new Guid(_webServerOptions.MutexName), () => WindowHelper.BringWindowToFront(Process.GetCurrentProcess().MainWindowHandle)))
                    {
                        Environment.Exit(0);
                    }
                }
            }
        }

        private static void EnsureAppDomainShadowCopying()
        {
            if (!AppDomain.CurrentDomain.ShadowCopyFiles)
            {
                string assemblyFilePath;
                string directoryPath;
                string cacheFilePath;
                string configFilePath;

                AppDomain appDomain;
                AppDomainSetup appDomainSetup;

                assemblyFilePath = Assembly.GetExecutingAssembly().Location;
                directoryPath = Path.GetDirectoryName(assemblyFilePath);

                cacheFilePath = Path.Combine(_webServerOptions.BaseDirectoryPath, ".cache");
                configFilePath = $"{ assemblyFilePath }.config";

                appDomainSetup = new AppDomainSetup()
                {
                    CachePath = cacheFilePath,
                    ConfigurationFile = configFilePath,
                    ShadowCopyDirectories = Path.Combine(_webServerOptions.BaseDirectoryPath, "plugin"),
                    ShadowCopyFiles = "true"
                };

                appDomain = AppDomain.CreateDomain("ShadowCopyTest", AppDomain.CurrentDomain.Evidence, appDomainSetup);
                appDomain.ExecuteAssembly(assemblyFilePath);
                AppDomain.Unload(appDomain);
                //Directory.Delete(cacheFilePath, true);
                Environment.Exit(0);
            }
        }

        public static ServiceControllerStatus GetOneDasServiceStatus()
        {
            if (_oneDasServiceController == null)
            {
                _oneDasServiceController = ServiceController.GetServices().FirstOrDefault(x => x.ServiceName == _webServerOptions.ServiceName);
            }

            if (_oneDasServiceController != null)
            {
                _oneDasServiceController.Refresh();

                return _oneDasServiceController.Status;
            }
            else
            {
                return 0;
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            BasicBootloader.SystemLogger.LogCritical(e.ExceptionObject.ToString());
        }

        #endregion

        #region "Properties"

        public static ILogger SystemLogger { get; private set; }

        #endregion

        #region "Shutdown"

        public static void Shutdown()
        {
            _advancedBootloader?.Dispose();
        }

        public static void Shutdown(int returnCode)
        {
            _advancedBootloader?.Dispose();

            Environment.Exit(returnCode);
        }

        #endregion
    }
}
