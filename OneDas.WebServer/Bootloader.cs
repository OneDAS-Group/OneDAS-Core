using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using OneDas.Common;
using OneDas.Engine.Core;
using OneDas.Engine.Serialization;
using OneDas.Infrastructure;
using OneDas.Plugin;
using OneDas.WebServer.Core;
using OneDas.WebServer.Logging;
using OneDas.WebServer.Shell;
using OneDas.WebServer.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace OneDas.WebServer
{
    public static class Bootloader
    {
        #region "Fields"

        private static OneDasEngine _oneDasEngine;
        private static ServiceController _oneDasServiceController;
        private static WebServerOptions _webServerOptions;
        private static IWebHost _webhost;
        private static IServiceCollection _serviceCollection;
        private static IServiceProvider _serviceProvider;
        private static IConfigurationRoot _configurationRoot;

        #endregion

        #region "Properties"

        public static ILogger SystemLogger { get; private set; }

        #endregion

        #region "Methods"

        public static void Main(string[] args)
        {
            IConfigurationBuilder configurationBuilder;
            string configurationDirectoryPath;

            // configuration
            configurationDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OneDAS");

            Directory.CreateDirectory(configurationDirectoryPath);

            configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.SetBasePath(configurationDirectoryPath);
            configurationBuilder.AddJsonFile("onedassettings.json");

            _configurationRoot = configurationBuilder.Build();
            _webServerOptions = _configurationRoot.Get<WebServerOptions>();

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
                _webServerOptions.Save();
            }

            // exception handling
            AppDomain.CurrentDomain.UnhandledException += Bootloader.CurrentDomain_UnhandledException;

            // parse command line
            Bootloader.ParseCommandLineArgumentSet(args);

            // ensure AppDomain shadow copying
            Bootloader.EnsureAppDomainShadowCopying();

            // dependency injection
            _serviceCollection = new ServiceCollection();

            Bootloader.ConfigureServices(_serviceCollection);
            _serviceProvider = _serviceCollection.BuildServiceProvider();

            // logging
            Bootloader.SystemLogger = _serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("System");

            // OneDasServiceController
            _oneDasServiceController = ServiceController.GetServices().FirstOrDefault(serviceController =>
            {
                return serviceController.ServiceName == _webServerOptions.ServiceName;
            });

            // OneDasConsole or OneDasService
            if (Environment.UserInteractive)
            {
                if (Bootloader.GetOneDasServiceStatus() == 0)
                {
                    _webhost = Bootloader.CreateWebHost();
                    _oneDasEngine = _serviceProvider.GetRequiredService<OneDasEngine>();

                    Task.Run(() =>
                    {
                        using (IWebHost webHost = _webhost)
                        {
                            _webhost.Run();
                        }
                    });

                    Bootloader.KickStartOneDasEngine();

                    new OneDasConsole(true).Run();
                }
                else
                {
                    new OneDasConsole(false).Run();
                }
            }
            else
            {
                _webServerOptions.IsAutostartEnabled = true;

                _webhost = Bootloader.CreateWebHost();
                _oneDasEngine = _serviceProvider.GetRequiredService<OneDasEngine>();

                Bootloader.KickStartOneDasEngine();

                _webhost.RunAsService();
            }
        }

        private static void KickStartOneDasEngine()
        {
            if (_webServerOptions.IsAutostartEnabled && File.Exists(_webServerOptions.CurrentProjectFilePath))
            {
                Task.Run(() =>
                {
                    try
                    {
                        _oneDasEngine.ActivateProject(ProjectSerializationHelper.Load(_webServerOptions.CurrentProjectFilePath), 4);
                        _oneDasEngine.OneDasState = OneDasState.Run;
                    }
                    catch (Exception ex)
                    {
                        _oneDasEngine.HandleException(ex);
                    }
                });
            }
        }

        private static void ParseCommandLineArgumentSet(IList<string> commandLineArgumentSet)
        {
            Contract.Requires(commandLineArgumentSet != null);

            if (!commandLineArgumentSet.Contains("-ALLOWSINGELTON"))
            {
                if (!(Environment.UserInteractive && Bootloader.GetOneDasServiceStatus() > 0))
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

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddLogging(loggingBuilder =>
            {
                EventLogSettings eventLogSettings;
                ClientMessageLoggerProvider clientMessageLoggerProvider;

                // WebServerOptions
                serviceCollection.Configure<WebServerOptions>(_configurationRoot);

                // event log
                if (Environment.UserInteractive)
                {
                    eventLogSettings = new EventLogSettings();
                }
                else
                {
                    eventLogSettings = new EventLogSettings()
                    {
                        LogName = _webServerOptions.EventLogName,
                        SourceName = _webServerOptions.EventLogSourceName
                    };
                }

                eventLogSettings.Filter = (category, logLevel) => category == "System";

                // client message log
                clientMessageLoggerProvider = new ClientMessageLoggerProvider((category, logLevel) => category != "System");

                // add logger
                loggingBuilder.AddEventLog(eventLogSettings);
                loggingBuilder.AddProvider(clientMessageLoggerProvider);
            });

            serviceCollection.AddOneDas(oneDasOptions =>
            {
                oneDasOptions.BaseDirectoryPath = _webServerOptions.BaseDirectoryPath;
            });

            serviceCollection.AddSingleton<OneDasConsole>(serviceProvider => new OneDasConsole(Bootloader.GetOneDasServiceStatus() == 0));
        }

        #endregion

        #region "Exception handling"
     
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Bootloader.SystemLogger.LogCritical(e.ExceptionObject.ToString());
        }

        #endregion

        #region "WebHost"

        private static IWebHost CreateWebHost()
        {
            IWebHost webHost;

            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")))
            {
                Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            }

            webHost = new WebHostBuilder()
                .ConfigureServices(serviceCollection => Bootloader.ConfigureServices(serviceCollection))
                .UseKestrel()
                .UseUrls(_webServerOptions.AspBaseUrl)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            return webHost;
        }

        #endregion

        #region "Shutdown"

        public static void Shutdown(int returnCode)
        {
            Bootloader.Dispose();

            Environment.Exit(returnCode);
        }

        #endregion

        #region "IDisposable Support"

        private static bool isDisposed;

        public static async void Dispose()
        {
            if (!isDisposed)
            {
                if (_oneDasEngine != null)
                {
                    Bootloader.SystemLogger.LogInformation("shutdown started");

                    await _webhost?.StopAsync();

                    _oneDasEngine.Dispose();

                    Bootloader.SystemLogger.LogInformation("shutdown finished");
                }
            }

            isDisposed = true;
        }

        #endregion
    }
}