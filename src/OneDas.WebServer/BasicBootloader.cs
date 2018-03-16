using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
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
            string configurationFileName;
            IConfigurationRoot configurationRoot;
            IConfigurationBuilder configurationBuilder;

            // configuration
            BasicBootloader.ConfigurationDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OneDAS", "Core");
            configurationFileName = "settings.json";

            Directory.CreateDirectory(BasicBootloader.ConfigurationDirectoryPath);

            configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile(new PhysicalFileProvider(BasicBootloader.ConfigurationDirectoryPath), path: configurationFileName, optional: true, reloadOnChange: true);
            configurationRoot = configurationBuilder.Build();

            _webServerOptions = configurationRoot.Get<WebServerOptions>();

            if (_webServerOptions == null)
            {
                _webServerOptions = new WebServerOptions();
                configurationRoot.Bind(_webServerOptions);
            }

            if (!Directory.Exists(_webServerOptions.BaseDirectoryPath))
            {
                _webServerOptions.BaseDirectoryPath = BasicBootloader.ConfigurationDirectoryPath;
            }

            if (string.IsNullOrWhiteSpace(_webServerOptions.NewBaseDirectoryPath))
            {
                _webServerOptions.NewBaseDirectoryPath = _webServerOptions.BaseDirectoryPath;
            }
            else if (!string.Equals(_webServerOptions.BaseDirectoryPath, _webServerOptions.NewBaseDirectoryPath, StringComparison.OrdinalIgnoreCase))
            {
                _webServerOptions.BaseDirectoryPath = _webServerOptions.NewBaseDirectoryPath;
            }

            _webServerOptions.Save(BasicBootloader.ConfigurationDirectoryPath);

            // determine startup mode
            if (Environment.UserInteractive && BasicBootloader.GetOneDasServiceStatus() > 0)
            {
                isHosting = false;
            }
            else
            {
                if (!WebServerUtilities.EnsureSingeltonInstance(new Guid(_webServerOptions.MutexName), () => WebServerUtilities.BringWindowToFront(Process.GetCurrentProcess().MainWindowHandle)))
                {
                    Environment.Exit(0);
                }

                isHosting = true;
            }

            // ensure AppDomain shadow copying
            BasicBootloader.EnsureAppDomainShadowCopying();

            // create advanced bootloader
            _advancedBootloader = new AdvancedBootloader(isHosting, _webServerOptions, configurationRoot);

            // logging
            BasicBootloader.SystemLogger = _advancedBootloader.CreateSystemLogger();

            // exception handling
            AppDomain.CurrentDomain.UnhandledException += BasicBootloader.CurrentDomain_UnhandledException;

            // run
            _advancedBootloader.Run();
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

		public static string ConfigurationDirectoryPath { get; private set; }
		
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
