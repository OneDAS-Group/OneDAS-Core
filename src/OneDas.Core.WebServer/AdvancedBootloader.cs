using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
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
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using RuntimeEnvironment = Microsoft.DotNet.PlatformAbstractions.RuntimeEnvironment;

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

                // client push service
                AdvancedBootloader.ClientPushService = _serviceProvider.GetRequiredService<ClientPushService>();

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
        }

        #endregion

        #region "Properties"

        public static ClientPushService ClientPushService { get; private set; }

        #endregion

        #region "Methods"

        public void Run()
        {
            OneDasConsole console;

            if (_engine != null && !BasicBootloader.IsUserInteractive)
            {
                this.TryStartOneDasEngine(_engine, _webServerOptions.CurrentProjectFilePath);
            }

            if (BasicBootloader.IsUserInteractive)
            {
                BasicBootloader.SystemLogger.LogInformation("started in user interactive mode (console)");

                console = _serviceProvider.GetRequiredService<OneDasConsole>();

                if (_webhost != null)
                {
                    console.RunAsync(_isHosting);
                    _webhost.Run();
                }
                else
                {
                    console.RunAsync(_isHosting).Wait();
                }
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
                WebClientLoggerProvider clientMessageLoggerProvider;

                // client message log
                clientMessageLoggerProvider = new WebClientLoggerProvider((category, logLevel) => category != "System" && logLevel >= LogLevel.Information);

                // add logger
                loggingBuilder
                    .AddFilter((provider, source, logLevel) => !source.StartsWith("Microsoft."))
                    .AddDebug()
                    .AddProvider(clientMessageLoggerProvider);

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    EventLogSettings eventLogSettings;

                    if (BasicBootloader.IsUserInteractive)
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

                    loggingBuilder.AddEventLog(eventLogSettings);
                }
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
