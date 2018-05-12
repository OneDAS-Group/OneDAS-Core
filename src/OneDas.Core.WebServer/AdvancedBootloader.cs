using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using Microsoft.Extensions.Options;
using OneDas.Core.Engine;
using OneDas.Core.Serialization;
using OneDas.WebServer.Core;
using OneDas.WebServer.Logging;
using OneDas.WebServer.Shell;
using OneDas.WebServer.Web;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OneDas.WebServer
{
    public class AdvancedBootloader : IDisposable
    {
        #region "Fields"

        private bool _isHosting;
        private OneDasEngine _engine;
        private WebServerOptions _webServerOptions;
        private IWebHost _webhost;
        private IConfiguration _configuration;
        private IServiceProvider _serviceProvider;
        private IServiceCollection _serviceCollection;

        #endregion

        #region "Constructors"

        public AdvancedBootloader(bool isHosting, WebServerOptions webServerOptions, IConfiguration configuration)
        {
            OneDasOptions options;
            ExtensionLoader extensionLoader;

            _isHosting = isHosting;
            _webServerOptions = webServerOptions;
            _configuration = configuration;

            if (isHosting)
            {
                _webhost = this.CreateWebHost();
                _serviceProvider = _webhost.Services;

                // create directories
                options = _serviceProvider.GetRequiredService<IOptions<OneDasOptions>>().Value;

                Directory.CreateDirectory(options.BackupDirectoryPath);
                Directory.CreateDirectory(options.ConfigurationDirectoryPath);
                Directory.CreateDirectory(options.DataDirectoryPath);
                Directory.CreateDirectory(options.NugetDirectoryPath);
                Directory.CreateDirectory(options.ProjectDirectoryPath);

                // extension loader
                extensionLoader = _serviceProvider.GetRequiredService<ExtensionLoader>();
                extensionLoader.ReloadPackages();

                // create engine
                _engine = _serviceProvider.GetRequiredService<OneDasEngine>();
            }
            else
            {
                _serviceCollection = new ServiceCollection();
                this.ConfigureServices(_serviceCollection);
                _serviceProvider = _serviceCollection.BuildServiceProvider();
            }

            AdvancedBootloader.ClientPushService = _serviceProvider.GetRequiredService<ClientPushService>();
        }

        #endregion

        #region "Properties"

        public static ClientPushService ClientPushService { get; private set; }

        #endregion

        #region "Methods"

        public void Run()
        {
            OneDasConsole console;

            if (_engine != null && !Environment.UserInteractive)
            {
                this.TryStartOneDasEngine(_engine, _webServerOptions.CurrentProjectFilePath);
            }

            if (Environment.UserInteractive)
            {
                BasicBootloader.SystemLogger.LogInformation("started in user interactive mode (console)");

                console = _serviceProvider.GetRequiredService<OneDasConsole>();

                _webhost?.StartAsync();
                console.Run(_isHosting);
            }
            else
            {
                BasicBootloader.SystemLogger.LogInformation("started in non-interactive mode (service)");

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
                .SuppressStatusMessages(true)
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
                WebClientLoggerProvider clientMessageLoggerProvider;

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

                eventLogSettings.Filter = (category, logLevel) => category == "System" && logLevel >= LogLevel.Information;

                // client message log
                clientMessageLoggerProvider = new WebClientLoggerProvider((category, logLevel) => category != "System" && logLevel >= LogLevel.Information);

                // add logger
                loggingBuilder.AddFilter((provider, source, logLevel) => !source.StartsWith("Microsoft."))
                    .AddDebug()
                    .AddEventLog(eventLogSettings)
                    .AddProvider(clientMessageLoggerProvider);
            });

            // OneDasEngine
            serviceCollection.AddOneDas(options =>
            {
                options.RestoreRuntimeId = RuntimeEnvironment.GetRuntimeIdentifier();
            });

            // Misc
            serviceCollection.AddSingleton<OneDasConsole>();
            serviceCollection.AddSingleton<ExtensionLoader>();
            serviceCollection.AddSingleton<ClientPushService>();
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
                        oneDasEngine.ActivateProject(oneDasProjectSerializer.Load(projectFilePath), 4);
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
                _engine?.Dispose();
            }

            isDisposed = true;
        }

        #endregion
    }
}