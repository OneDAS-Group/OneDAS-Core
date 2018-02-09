using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneDas.Common;
using OneDas.Engine.Core;
using OneDas.Engine.Serialization;
using OneDas.Infrastructure;
using OneDas.Plugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OneDas.WebServer.Web
{
    public class WebClientHub : Hub<IWebClientHub>
    {
        private IPluginProvider _pluginProvider;
        private OneDasEngine _oneDasEngine;
        private WebServerOptions _webServerOptions;

        private ILogger _webServerLogger;
        private IOneDasProjectSerializer _projectSerializer;

        public WebClientHub(IPluginProvider pluginProvider, OneDasEngine oneDasEngine, IOptions<WebServerOptions> options, ILoggerFactory loggerFactory, IOneDasProjectSerializer oneDasProjectSerializer)
        {
            _pluginProvider = pluginProvider;
            _oneDasEngine = oneDasEngine;
            _webServerOptions = options.Value;
            _webServerLogger = loggerFactory.CreateLogger("WebServer");
            _projectSerializer = oneDasProjectSerializer;
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (HomeController.LiveViewSubscriptionSet.ContainsKey(this.Context.ConnectionId))
            {
                HomeController.LiveViewSubscriptionSet.Remove(this.Context.ConnectionId);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public Task StartOneDas()
        {
            return Task.Run(() => _oneDasEngine.Start());
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
                _webServerOptions.Save(_webServerOptions.BaseDirectoryPath);

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
                _webServerOptions.Save(_webServerOptions.BaseDirectoryPath);

                _oneDasEngine.ActivateProject(projectSettings, 2);

                this.Clients.All.SendActiveProject(projectSettings);
            });
        }

        public Task<int> UpdateLiveViewSubscription(IList<Guid> channelHubGuidSet)
        {
            int subscriptionId;
            IList<ChannelHub> channelHubSet;

            return Task.Run(() =>
            {
                try
                {
                    channelHubSet = channelHubGuidSet.Select(channelHubGuid => _oneDasEngine.Project.ActiveChannelHubSet.First(channelHub => channelHub.Guid == channelHubGuid)).ToList();
                }
                catch (Exception)
                {
                    throw new Exception(ErrorMessage.WebClientHub_ChannelHubNotFound);
                }

                subscriptionId = HomeController.GetNextSubscriptionId();
                HomeController.LiveViewSubscriptionSet[this.Context.ConnectionId] = (subscriptionId, channelHubSet);

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
                IList<DataGatewayPluginSettingsBase> dataGatewayPluginSettingsSet;
                IList<DataWriterPluginSettingsBase> dataWriterPluginSettingsSet;

                dataGatewayPluginSettingsSet = Enumerable.
                        Where(_pluginProvider.Get<DataGatewayPluginSettingsBase>(), pluginSettingsType => pluginSettingsType.GetFirstAttribute<PluginIdentificationAttribute>().Id == "EtherCAT").
                        Select(pluginSettingsType => (DataGatewayPluginSettingsBase)Activator.CreateInstance(pluginSettingsType)).ToList();

                dataWriterPluginSettingsSet = Enumerable.
                        Where(_pluginProvider.Get<DataWriterPluginSettingsBase>(), pluginSettingsType => pluginSettingsType.GetFirstAttribute<PluginIdentificationAttribute>().Id == "HDF").
                        Select(pluginSettingsType => (DataWriterPluginSettingsBase)Activator.CreateInstance(pluginSettingsType)).ToList();

                return new OneDasProjectSettings(
                    primaryGroupName,
                    SecondaryGroupName, 
                    configurationName,
                    dataGatewayPluginSettingsSet,
                    dataWriterPluginSettingsSet);
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
    }
}
