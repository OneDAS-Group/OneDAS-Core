using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneDas.Common;
using OneDas.Engine.Core;
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
                fileName = $"{projectSettings.Description.CampaignPrimaryGroup}_{projectSettings.Description.CampaignSecondaryGroup}_{projectSettings.Description.CampaignName}_{projectSettings.Description.Guid.ToString()}.json";
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

        public Task ActivateProject(OneDasProjectDescription projectDescription)
        {
            string filePath;
            OneDasProjectSettings projectSettings;

            return Task.Run(() =>
            {
                projectDescription.Validate();

                // Improve: Make more flexible, renaming of file is impossible like that
                filePath = Path.Combine(_webServerOptions.BaseDirectoryPath, "project", $"{ projectDescription.CampaignPrimaryGroup }_{ projectDescription.CampaignSecondaryGroup }_{ projectDescription.CampaignName }_{ projectDescription.Guid }.json");
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

        public Task<IEnumerable<OneDasProjectDescription>> GetProjectDescriptions()
        {
            return Task.Run(() =>
            {
                IEnumerable<string> filePathSet;
                IList<OneDasProjectDescription> projectDescriptionSet;

                filePathSet = Directory.GetFiles(Path.Combine(_webServerOptions.BaseDirectoryPath, "project"), "*.json");
                projectDescriptionSet = new List<OneDasProjectDescription>();

                foreach (string filePath in filePathSet)
                {
                    try
                    {
                        projectDescriptionSet.Add(_projectSerializer.GetProjectDescriptionFromFile(filePath));
                    }
                    catch (Exception)
                    {
                        //
                    }
                }

                return (IEnumerable<OneDasProjectDescription>)projectDescriptionSet;
            });
        }

        public Task<OneDasProjectSettings> OpenProject(OneDasProjectDescription projectDescription)
        {
            return Task.Run(() =>
            {
                // Improve: Make more flexible, renaming of file is impossible like that
                OneDasProjectSettings projectSettings = _projectSerializer.Load(Path.Combine(_webServerOptions.BaseDirectoryPath, "project", $"{ projectDescription.CampaignPrimaryGroup }_{ projectDescription.CampaignSecondaryGroup }_{ projectDescription.CampaignName }_{ projectDescription.Guid }.json"));

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
                    activeProjectSettings: _oneDasEngine.Project.Settings,
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

        public Task<OneDasProjectSettings> CreateProject(string campaignPrimaryGroup, string campaignSecondaryGroup, string configurationName)
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
                    campaignPrimaryGroup, 
                    campaignSecondaryGroup, 
                    configurationName,
                    dataGatewayPluginSettingsSet,
                    dataWriterPluginSettingsSet);
            });
        }

        public Task<DataGatewayPluginSettingsBase> CreateDataGatewaySettings(string pluginName)
        {
            return Task.Run(() =>
            {
                return (DataGatewayPluginSettingsBase)Activator.CreateInstance(_pluginProvider.Get(pluginName));
            });
        }

        public Task<DataWriterPluginSettingsBase> CreateDataWriterSettings(string pluginName)
        {
            return Task.Run(() =>
            {
                return (DataWriterPluginSettingsBase)Activator.CreateInstance(_pluginProvider.Get(pluginName));
            });
        }
    }
}
