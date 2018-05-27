using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;

namespace OneDas.WebServer
{
    public static class BasicBootloader
    {
        #region "Fields"

        private static WebServerOptions _options;
        private static ServiceController _serviceController;
        private static AdvancedBootloader _advancedBootloader;

        #endregion

        #region "Methods"

        public static void Main(string[] args)
        {
            bool isHosting;
            string configurationFileName;          
            IConfigurationRoot configurationRoot;
            IConfigurationBuilder configurationBuilder;

            Thread.CurrentThread.Name = "Main thread";
            BasicBootloader.IsUserInteractive = !args.Contains("--non-interactive");

            // configuration
            BasicBootloader.ConfigurationDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OneDAS", "Core");
            configurationFileName = "settings.json";

            Directory.CreateDirectory(BasicBootloader.ConfigurationDirectoryPath);

            configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile(new PhysicalFileProvider(BasicBootloader.ConfigurationDirectoryPath), path: configurationFileName, optional: true, reloadOnChange: true);
            configurationRoot = configurationBuilder.Build();

            _options = configurationRoot.Get<WebServerOptions>();

            if (_options == null)
            {
                _options = new WebServerOptions();
                configurationRoot.Bind(_options);
            }

            if (!Directory.Exists(_options.BaseDirectoryPath))
            {
                _options.BaseDirectoryPath = BasicBootloader.ConfigurationDirectoryPath;
            }

            if (string.IsNullOrWhiteSpace(_options.NewBaseDirectoryPath))
            {
                _options.NewBaseDirectoryPath = _options.BaseDirectoryPath;
            }
            else if (!string.Equals(_options.BaseDirectoryPath, _options.NewBaseDirectoryPath, StringComparison.OrdinalIgnoreCase))
            {
                _options.BaseDirectoryPath = _options.NewBaseDirectoryPath;
            }

            _options.Save(BasicBootloader.ConfigurationDirectoryPath);

            // determine startup mode
            if (BasicBootloader.IsUserInteractive && BasicBootloader.GetOneDasServiceStatus() > 0)
            {
                isHosting = false;
            }
            else
            {
                if (!WebServerUtilities.EnsureSingeltonInstance(new Guid(_options.MutexName)))
                {
                    Environment.Exit(0);
                }

                isHosting = true;
            }

            // create advanced bootloader
            _advancedBootloader = new AdvancedBootloader(isHosting, _options, configurationRoot);

            // logging
            BasicBootloader.SystemLogger = _advancedBootloader.CreateSystemLogger();

            // exception handling
            AppDomain.CurrentDomain.UnhandledException += BasicBootloader.CurrentDomain_UnhandledException;

            // run
            _advancedBootloader.Run();

            // shutdown 
            BasicBootloader.Shutdown(0);
        }

        public static ServiceControllerStatus GetOneDasServiceStatus()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return 0;
            }

            if (_serviceController == null)
            {
                _serviceController = ServiceController.GetServices().FirstOrDefault(x => x.ServiceName == _options.ServiceName);
            }

            if (_serviceController != null)
            {
                _serviceController.Refresh();

                return _serviceController.Status;
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

        private static void Shutdown(int returnCode)
        {
            _advancedBootloader?.Dispose();

            Environment.Exit(returnCode);
        }

        #endregion

        #region "Properties"

        public static bool IsUserInteractive { get; private set; }
        public static string ConfigurationDirectoryPath { get; private set; }
        public static ILogger SystemLogger { get; private set; }
        public static JObject NugetFramework { get; private set; }

        #endregion
    }
}
