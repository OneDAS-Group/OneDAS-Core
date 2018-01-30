using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using OneDas.Engine.Core;
using OneDas.Infrastructure;
using OneDas.Plugin;
using OneDas.WebServer.Logging;
using OneDas.WebServer.Shell;
using OneDas.WebServer.Web;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace OneDas.WebServer
{
    public class AdvancedBootloader : IDisposable
    {
        #region "Fields"

        private bool _isHosting;
        private OneDasEngine _oneDasEngine;
        private WebServerOptions _webServerOptions;
        private IPluginProvider _pluginProvider;
        private IWebHost _webhost;
        private IConfiguration _configuration;
        private IServiceProvider _serviceProvider;
        private IServiceCollection _serviceCollection;

        #endregion

        #region "Constructors"

        public AdvancedBootloader(bool isHosting, WebServerOptions webServerOptions, IConfiguration configuration)
        {
            Version minimumVersion;

            _isHosting = isHosting;
            _webServerOptions = webServerOptions;
            _configuration = configuration;

            if (isHosting)
            {
                _webhost = this.CreateWebHost();
                _serviceProvider = _webhost.Services;
                _pluginProvider = _serviceProvider.GetRequiredService<IPluginProvider>();

                // create directories
                Directory.CreateDirectory(_webServerOptions.BaseDirectoryPath);
                Directory.CreateDirectory(Path.Combine(_webServerOptions.BaseDirectoryPath, "backup"));
                Directory.CreateDirectory(Path.Combine(_webServerOptions.BaseDirectoryPath, "data"));
                Directory.CreateDirectory(Path.Combine(_webServerOptions.BaseDirectoryPath, "plugin"));
                Directory.CreateDirectory(Path.Combine(_webServerOptions.BaseDirectoryPath, "project"));

                // load plugins
                minimumVersion = new Version(new Version(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion).Major, 0, 0, 0);

                _pluginProvider = _serviceProvider.GetRequiredService<IPluginProvider>();
                _pluginProvider.ScanAssemblies(Path.Combine(_webServerOptions.BaseDirectoryPath, "plugin"), "OneDAS", minimumVersion);

                // create engine
                _oneDasEngine = _serviceProvider.GetRequiredService<OneDasEngine>();
            }
            else
            {
                _serviceCollection = new ServiceCollection();
                this.ConfigureServices(_serviceCollection);
                _serviceProvider = _serviceCollection.BuildServiceProvider();
            }
        }

        #endregion

        #region "Methods"

        public void Run()
        {
            OneDasConsole oneDasConsole;

            if (_oneDasEngine != null && !Environment.UserInteractive)
            {
                this.TryStartOneDasEngine(_oneDasEngine, _webServerOptions.CurrentProjectFilePath);
            }

            if (Environment.UserInteractive)
            {
                oneDasConsole = _serviceProvider.GetRequiredService<OneDasConsole>();

                _webhost?.StartAsync();
                oneDasConsole.Run(_isHosting);
            }
            else
            {
                _webhost?.RunAsService();
            }
        }

        public ILogger CreateSystemLogger()
        {
            return _serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("System");
        }

        private IWebHost CreateWebHost()
        {
            IWebHost webHost;

            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")))
            {
                Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            }

            webHost = new WebHostBuilder()
                .ConfigureServices(serviceCollection => this.ConfigureServices(serviceCollection))
                .UseKestrel()
                .UseUrls(_webServerOptions.AspBaseUrl)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            return webHost;
        }

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;

            // WebServerOptions
            serviceCollection.Configure<WebServerOptions>(_configuration);

            // logging
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
                loggingBuilder.AddFilter((provider, source, logLevel) => !source.StartsWith("Microsoft."));
            });

            // OneDAS Engine
            serviceCollection.AddOneDas(oneDasOptions =>
            {
                oneDasOptions.DataDirectoryPath = Path.Combine(_webServerOptions.BaseDirectoryPath, "data");
            });

            // OneDasConsole
            serviceCollection.AddSingleton<OneDasConsole>();
        }

        private void TryStartOneDasEngine(OneDasEngine oneDasEngine, string projectFilePath)
        {
            if (File.Exists(projectFilePath))
            {
                Task.Run(() =>
                {
                    IOneDasProjectSerializer oneDasProjectSerializer;

                    oneDasProjectSerializer = _serviceProvider.GetRequiredService<IOneDasProjectSerializer>();

                    try
                    {
                        oneDasEngine.ActivateProject(oneDasProjectSerializer.Load(_webServerOptions.CurrentProjectFilePath), 4);
                        oneDasEngine.Start();
                    }
                    catch (Exception ex)
                    {
                        oneDasEngine.HandleException(ex);
                    }
                });
            }
        }

        #endregion

        #region "IDisposable Support"

        private bool isDisposed;

        public void Dispose()
        {
            if (!isDisposed)
            {
                _webhost?.StopAsync().Wait();
                _oneDasEngine?.Dispose();
            }

            isDisposed = true;
        }

        #endregion
    }
}
