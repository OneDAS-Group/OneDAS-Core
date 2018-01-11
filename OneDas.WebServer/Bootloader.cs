using Microsoft.AspNetCore.Hosting;
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
        private static IWebHost _webhost;
        private static IServiceCollection _serviceCollection;
        private static IServiceProvider _serviceProvider;

        #endregion

        #region "Properties"

        public static ILogger SystemLogger { get; private set; }

        #endregion

        #region "Methods"

        public static void Main(string[] args)
        {
            string configurationDirectoryPath;

            // configuration
            configurationDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OneDAS");

            Directory.CreateDirectory(configurationDirectoryPath);

            ConfigurationManager<WebServerOptions>.Initialize(configurationDirectoryPath, "onedassettings.json");

            // base directory path
            if (!Directory.Exists(ConfigurationManager<WebServerOptions>.Options.BaseDirectoryPath))
            {
                ConfigurationManager<WebServerOptions>.Options.BaseDirectoryPath = configurationDirectoryPath;
            }

            try
            {
                if (!string.Equals(Path.GetFullPath(ConfigurationManager<WebServerOptions>.Options.BaseDirectoryPath), Path.GetFullPath(ConfigurationManager<WebServerOptions>.Options.NewBaseDirectoryPath), StringComparison.OrdinalIgnoreCase))
                {
                    ConfigurationManager<WebServerOptions>.Options.BaseDirectoryPath = ConfigurationManager<WebServerOptions>.Options.NewBaseDirectoryPath;
                }
            }
            catch
            {
                ConfigurationManager<WebServerOptions>.Options.NewBaseDirectoryPath = ConfigurationManager<WebServerOptions>.Options.BaseDirectoryPath;
            }
            finally
            {
                ConfigurationManager<WebServerOptions>.Save();
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
                return serviceController.ServiceName == ConfigurationManager<WebServerOptions>.Options.ServiceName;
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
                ConfigurationManager<WebServerOptions>.Options.IsAutostartEnabled = true;

                _webhost = Bootloader.CreateWebHost();
                _oneDasEngine = _serviceProvider.GetRequiredService<OneDasEngine>();

                Bootloader.KickStartOneDasEngine();

                using (OneDasService oneDasService = new OneDasService(ConfigurationManager<WebServerOptions>.Options.ServiceName, _webhost))
                {
                    ServiceBase.Run(oneDasService);
                }
            }
        }

        private static void KickStartOneDasEngine()
        {
            if (ConfigurationManager<WebServerOptions>.Options.IsAutostartEnabled && File.Exists(ConfigurationManager<WebServerOptions>.Options.CurrentProjectFilePath))
            {
                Task.Run(() =>
                {
                    try
                    {
                        _oneDasEngine.ActivateProject(ProjectSerializationHelper.Load(ConfigurationManager<WebServerOptions>.Options.CurrentProjectFilePath), 4);
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
                    if (!SingeltonHelper.EnsureSingeltonInstance(new Guid(ConfigurationManager<WebServerOptions>.Options.MutexName), () => WindowHelper.BringWindowToFront(Process.GetCurrentProcess().MainWindowHandle)))
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

                cacheFilePath = Path.Combine(ConfigurationManager<WebServerOptions>.Options.BaseDirectoryPath, ".cache");
                configFilePath = $"{ assemblyFilePath }.config";

                appDomainSetup = new AppDomainSetup()
                {
                    CachePath = cacheFilePath,
                    ConfigurationFile = configFilePath,
                    ShadowCopyDirectories = Path.Combine(ConfigurationManager<WebServerOptions>.Options.BaseDirectoryPath, "plugin"),
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
                _oneDasServiceController = ServiceController.GetServices().FirstOrDefault(x => x.ServiceName == ConfigurationManager<WebServerOptions>.Options.ServiceName);
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

                // event log
                if (Environment.UserInteractive)
                {
                    eventLogSettings = new EventLogSettings();
                }
                else
                {
                    eventLogSettings = new EventLogSettings()
                    {
                        LogName = ConfigurationManager<WebServerOptions>.Options.EventLogName,
                        SourceName = ConfigurationManager<WebServerOptions>.Options.EventLogSourceName
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
                oneDasOptions.BaseDirectoryPath = ConfigurationManager<WebServerOptions>.Options.BaseDirectoryPath;
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
                .UseUrls(ConfigurationManager<WebServerOptions>.Options.AspBaseUrl)
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