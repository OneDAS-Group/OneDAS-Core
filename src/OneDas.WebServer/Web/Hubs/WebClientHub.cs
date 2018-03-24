using OneDas.Engine.Core;
using OneDas.Engine.Serialization;
using OneDas.Infrastructure;
using OneDas.Plugin;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;

namespace OneDas.WebServer.Web
{
    public class WebClientHub : Hub<IWebClientHub>
    {
        private IPluginProvider _pluginProvider;
        private WebServerOptions _webServerOptions;

        private ILogger _webServerLogger;
        private IOneDasProjectSerializer _projectSerializer;

        private static int _nextSubscriptionId;

        private static Timer _updatePerfInfoTimer;
        private static Timer _updateDataSnapshotTimer;
        private static Timer _updateLiveValueDataTimer;

        private static OneDasEngine _oneDasEngine;
        private static IHubContext<WebClientHub> _hubContext;

        public static Dictionary<string, (int SubscriptionId, IList<ChannelHubBase> ChannelHubSet)> LiveViewSubscriptionSet { get; private set; }

        static WebClientHub()
        {
            WebClientHub.LiveViewSubscriptionSet = new Dictionary<string, (int SubscriptionId, IList<ChannelHubBase> ChannelHubSet)>();

            _nextSubscriptionId = 1;
        }


        public WebClientHub(IPluginProvider pluginProvider, OneDasEngine oneDasEngine, IHubContext<WebClientHub> hubContext, IOptions<WebServerOptions> options, ILoggerFactory loggerFactory, IOneDasProjectSerializer oneDasProjectSerializer)
        {
            // setup callbacks
            if (_oneDasEngine == null)
            {
                _hubContext = hubContext;

                _oneDasEngine = oneDasEngine;
                _oneDasEngine.OneDasStateChanged += WebClientHub.OneDasEngine_OneDasStateChanged;

                _updatePerfInfoTimer = new Timer() { AutoReset = true, Enabled = true, Interval = TimeSpan.FromSeconds(1).TotalMilliseconds };
                _updateDataSnapshotTimer = new Timer() { AutoReset = true, Enabled = true, Interval = TimeSpan.FromSeconds(1).TotalMilliseconds };
                _updateLiveValueDataTimer = new Timer() { AutoReset = true, Enabled = true, Interval = TimeSpan.FromMilliseconds(200).TotalMilliseconds };

                _updatePerfInfoTimer.Elapsed += _updatePerfInfoTimer_Elapsed;
                _updateDataSnapshotTimer.Elapsed += _updateDataSnapshotTimer_Elapsed;
                _updateLiveValueDataTimer.Elapsed += _updateLiveValueDataTimer_Elapsed;
            }

            _pluginProvider = pluginProvider;
            _oneDasEngine = oneDasEngine;
            _webServerOptions = options.Value;
            _webServerLogger = loggerFactory.CreateLogger("WebServer");
            _projectSerializer = oneDasProjectSerializer;
        }

        #region "Methods"

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (WebClientHub.LiveViewSubscriptionSet.ContainsKey(this.Context.ConnectionId))
            {
                WebClientHub.LiveViewSubscriptionSet.Remove(this.Context.ConnectionId);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public Task StartOneDas()
        {
            return Task.Run(() => _oneDasEngine.Start());
        }

        public Task PauseOneDas()
        {
            return Task.Run(() => _oneDasEngine.Pause());
        }

        public Task StopOneDas()
        {
            return Task.Run(() => _oneDasEngine.Stop());
        }

        public Task AcknowledgeError()
        {
            return Task.Run(() => _oneDasEngine.AcknowledgeError());
        }

        public Task<string> GetLastError()
        {
            return Task.Run(() => _oneDasEngine.LastError);
        }

        public Task SaveWebServerOptionsLight(WebServerOptionsLight webServerOptionsLight)
        {
            Uri uri;
            Boolean isValidUri;

            return Task.Run(() =>
            {
                isValidUri = Uri.TryCreate(webServerOptionsLight.BaseDirectoryPath, UriKind.Absolute, out uri);

                if (!(isValidUri && uri.IsLoopback && Directory.Exists(webServerOptionsLight.BaseDirectoryPath)))
                {
                    throw new Exception(ErrorMessage.WebClientHub_BaseDirectoryPathIsInvalid);
                }

                _webServerOptions.OneDasName = webServerOptionsLight.OneDasName;
                _webServerOptions.AspBaseUrl = webServerOptionsLight.AspBaseUrl;
                _webServerOptions.NewBaseDirectoryPath = webServerOptionsLight.BaseDirectoryPath;
                _webServerOptions.Save(BasicBootloader.ConfigurationDirectoryPath);

                this.Clients.All.SendWebServerOptionsLight(webServerOptionsLight);
            });
        }

        public Task SaveProject(OneDasProjectSettings projectSettings)
        {
            string fileName;
            string directoryPath;
            string currentFilePath;
            string newFilePath;

            Contract.Requires(projectSettings != null);

            return Task.Run(() =>
            {
                projectSettings.Validate();

                directoryPath = Path.Combine(_webServerOptions.BaseDirectoryPath, "project");
                fileName = $"{ projectSettings.Description.PrimaryGroupName }_{ projectSettings.Description.SecondaryGroupName }_{ projectSettings.Description.CampaignName }_{ projectSettings.Description.Guid.ToString() }.json";
                currentFilePath = Path.Combine(directoryPath, fileName);

                try
                {
                    if (File.Exists(currentFilePath))
                    {
                        newFilePath = Path.Combine(_webServerOptions.BaseDirectoryPath, "backup", $"{ DateTime.UtcNow.ToString("yyyy-MM-ddTHH-mm-ss") }_{ fileName }");

                        File.Copy(currentFilePath, newFilePath, true);
                    }
                }
                finally
                {
                    _projectSerializer.Save(projectSettings, currentFilePath);
                    _webServerLogger.LogInformation("project file saved");
                }
            });
        }

        public Task ActivateProject(OneDasCampaignDescription campaignDescription)
        {
            string filePath;
            OneDasProjectSettings projectSettings;

            return Task.Run(() =>
            {
                campaignDescription.Validate();

                // Improve: Make more flexible, renaming of file is impossible like that
                filePath = Path.Combine(_webServerOptions.BaseDirectoryPath, "project", $"{ campaignDescription.PrimaryGroupName }_{ campaignDescription.SecondaryGroupName }_{ campaignDescription.CampaignName }_{ campaignDescription.Guid }.json");
                projectSettings = _projectSerializer.Load(filePath);

                _webServerOptions.CurrentProjectFilePath = filePath;
                _webServerOptions.Save(BasicBootloader.ConfigurationDirectoryPath);

                _oneDasEngine.ActivateProject(projectSettings, 2);

                this.Clients.All.SendActiveProject(projectSettings);
            });
        }

        public Task<int> UpdateLiveViewSubscription(IList<Guid> channelHubGuidSet)
        {
            int subscriptionId;
            IList<ChannelHubBase> channelHubSettingsSet;

            return Task.Run(() =>
            {
                try
                {
                    channelHubSettingsSet = channelHubGuidSet.Select(channelHubGuid => _oneDasEngine.Project.ActiveChannelHubSet.First(channelHub => channelHub.Settings.Guid == channelHubGuid)).ToList();
                }
                catch (Exception)
                {
                    throw new Exception(ErrorMessage.WebClientHub_ChannelHubNotFound);
                }

                subscriptionId = WebClientHub.GetNextSubscriptionId();
                WebClientHub.LiveViewSubscriptionSet[this.Context.ConnectionId] = (subscriptionId, channelHubSettingsSet);

                return subscriptionId;
            });
        }

        public Task<string> GetPluginStringResource(string pluginId, string resourceName)
        {
            return Task.Run(() =>
            {
                return _pluginProvider.GetStringResource(pluginId, resourceName);
            });
        }

        public Task<IEnumerable<OneDasCampaignDescription>> GetCampaignDescriptions()
        {
            return Task.Run(() =>
            {
                IEnumerable<string> filePathSet;
                IList<OneDasCampaignDescription> campaignDescriptionSet;

                filePathSet = Directory.GetFiles(Path.Combine(_webServerOptions.BaseDirectoryPath, "project"), "*.json");
                campaignDescriptionSet = new List<OneDasCampaignDescription>();

                foreach (string filePath in filePathSet)
                {
                    try
                    {
                        campaignDescriptionSet.Add(_projectSerializer.GetCampaignDescriptionFromFile(filePath));
                    }
                    catch (Exception)
                    {
                        //
                    }
                }

                return (IEnumerable<OneDasCampaignDescription>)campaignDescriptionSet;
            });
        }

        public Task<OneDasProjectSettings> OpenProject(OneDasCampaignDescription campaignDescription)
        {
            // Improve: Make more flexible, renaming of file is impossible like that
            return Task.Run(() =>
            {
                OneDasProjectSettings projectSettings;

                projectSettings = _projectSerializer.Load(Path.Combine(_webServerOptions.BaseDirectoryPath, "project", $"{ campaignDescription.PrimaryGroupName }_{ campaignDescription.SecondaryGroupName }_{ campaignDescription.CampaignName }_{ campaignDescription.Guid }.json"));

                return projectSettings;
            });
        }

        public Task<ActionResponse> RequestAction(ActionRequest actionRequest)
        {
            return Task.Run(() => _pluginProvider.HandleActionRequest(actionRequest));
        }

        public Task<AppModel> GetAppModel()
        {
            IList<PluginIdentificationAttribute> dataGatewayPluginIdentificationSet;
            IList<PluginIdentificationAttribute> dataWriterPluginIdentificationSet;

            string productVersion;

            productVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

            dataGatewayPluginIdentificationSet = _pluginProvider.Get<DataGatewayPluginSettingsBase>().Select(dataGatewaySettingsType =>
            {
                PluginIdentificationAttribute attribute;

                attribute = dataGatewaySettingsType.GetFirstAttribute<PluginIdentificationAttribute>();
                attribute.ProductVersion = FileVersionInfo.GetVersionInfo(dataGatewaySettingsType.Assembly.Location).ProductVersion;

                return attribute;
            }).ToList();

            dataWriterPluginIdentificationSet = _pluginProvider.Get<DataWriterPluginSettingsBase>().Select(dataWriterSettingsType =>
            {
                PluginIdentificationAttribute attribute;

                attribute = dataWriterSettingsType.GetFirstAttribute<PluginIdentificationAttribute>();
                attribute.ProductVersion = FileVersionInfo.GetVersionInfo(dataWriterSettingsType.Assembly.Location).ProductVersion;

                return attribute;
            }).ToList();

            return Task.Run(() =>
            {
                return new AppModel(
                    activeProjectSettings: _oneDasEngine.Project?.Settings,
                    clientSet: new List<string>() { },
                    dataGatewayPluginIdentificationSet: dataGatewayPluginIdentificationSet,
                    dataWriterPluginIdentificationSet: dataWriterPluginIdentificationSet,
                    productVersion: productVersion,
                    lastError: _oneDasEngine.LastError,
                    oneDasState: _oneDasEngine.OneDasState,
                    webServerOptionsLight: new WebServerOptionsLight
                    {
                        OneDasName = _webServerOptions.OneDasName,
                        AspBaseUrl = _webServerOptions.AspBaseUrl,
                        BaseDirectoryPath = _webServerOptions.BaseDirectoryPath
                    });
            });
        }

        public Task<OneDasProjectSettings> CreateProject(string primaryGroupName, string SecondaryGroupName, string configurationName)
        {
            return Task.Run(() =>
            {
                return new OneDasProjectSettings(
                    primaryGroupName,
                    SecondaryGroupName,
                    configurationName,
                    new List<DataGatewayPluginSettingsBase>(),
                    new List<DataWriterPluginSettingsBase>());
            });
        }

        public Task<DataGatewayPluginSettingsBase> CreateDataGatewaySettings(string pluginName)
        {
            return Task.Run(() =>
            {
                return (DataGatewayPluginSettingsBase)Activator.CreateInstance(_pluginProvider.GetSettings(pluginName));
            });
        }

        public Task<DataWriterPluginSettingsBase> CreateDataWriterSettings(string pluginName)
        {
            return Task.Run(() =>
            {
                return (DataWriterPluginSettingsBase)Activator.CreateInstance(_pluginProvider.GetSettings(pluginName));
            });
        }

        #endregion

        #region "Callbacks"

        public static void SendMessage(string message)
        {
            _hubContext?.Clients.All.SendAsync("SendMessage", message);
        }

        private static int GetNextSubscriptionId()
        {
            return _nextSubscriptionId++;
        }

        private static void _updatePerfInfoTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            OneDasPerformanceInformation performanceInformation;

            if (_oneDasEngine.OneDasState >= OneDasState.Ready)
            {
                performanceInformation = _oneDasEngine.CreatePerformanceInformation();
                _hubContext.Clients.All.SendAsync("SendPerformanceInformation", performanceInformation);
            }
        }

        private static void _updateDataSnapshotTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            IEnumerable<object> dataSnapshot;

            if (_oneDasEngine.OneDasState >= OneDasState.Ready)
            {
                dataSnapshot = _oneDasEngine.CreateDataSnapshot();
                _hubContext.Clients.All.SendAsync("SendDataSnapshot", DateTime.UtcNow, dataSnapshot);
            }
        }

        private static void _updateLiveValueDataTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            IEnumerable<object> dataSnapshot;

            if (_oneDasEngine.OneDasState >= OneDasState.Ready)
            {
                foreach (var liveViewSubscription in WebClientHub.LiveViewSubscriptionSet)
                {
                    dataSnapshot = _oneDasEngine.CreateDataSnapshot(liveViewSubscription.Value.ChannelHubSet);
                    _hubContext.Clients.Client(liveViewSubscription.Key).SendAsync("SendLiveViewData", liveViewSubscription.Value.SubscriptionId, DateTime.UtcNow, dataSnapshot);
                }
            }
        }

        private static void OneDasEngine_OneDasStateChanged(object sender, OneDasStateChangedEventArgs e)
        {
            _hubContext.Clients.All.SendAsync("SendOneDasState", e.NewState);
        }

        #endregion
    }
}
