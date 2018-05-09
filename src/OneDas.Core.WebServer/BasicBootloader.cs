using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;

namespace OneDas.WebServer
{
    public static class BasicBootloader
    {
        #region "Fields"

        private static WebServerOptions _options;
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

            WebServerUtilities.ModifyConsoleMenu(SystemCommand.SC_CLOSE, 0x0);

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
            if (Environment.UserInteractive && BasicBootloader.GetOneDasServiceStatus() > 0)
            {
                isHosting = false;
            }
            else
            {
                if (!WebServerUtilities.EnsureSingeltonInstance(new Guid(_options.MutexName), () => WebServerUtilities.BringWindowToFront(Process.GetCurrentProcess().MainWindowHandle)))
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
        }

        public static ServiceControllerStatus GetOneDasServiceStatus()
        {
            if (_oneDasServiceController == null)
            {
                _oneDasServiceController = ServiceController.GetServices().FirstOrDefault(x => x.ServiceName == _options.ServiceName);
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
        public static JObject NugetFramework { get; private set; }

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
