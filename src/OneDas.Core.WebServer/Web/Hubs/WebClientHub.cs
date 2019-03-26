using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneDas.Core.Engine;
using OneDas.Core.ProjectManagement;
using OneDas.Core.Serialization;
using OneDas.Extensibility;
using OneDas.PackageManagement;
using OneDas.Infrastructure;
using OneDas.WebServer.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace OneDas.WebServer.Web
{
    public class WebClientHub : Hub<IWebClientHub>
    {
        private OneDasEngine _engine;
        private OneDasPackageManager _packageManager;
        private ClientPushService _clientPushService;
        private WebServerOptions _webServerOptions;

        private IExtensionFactory _extensionFactory;
        private ILogger _webServerLogger;
        private IOneDasProjectSerializer _projectSerializer;

        public WebClientHub(
            OneDasEngine engine,
            OneDasPackageManager packageManager,
            ClientPushService clientPushService,
            IExtensionFactory extensionFactory, 
            ILoggerFactory loggerFactory, 
            IOneDasProjectSerializer projectSerializer,
            IOptions<WebServerOptions> options)
        {
            _engine = engine;
            _packageManager = packageManager;
            _clientPushService = clientPushService;
            _extensionFactory = extensionFactory;
            _webServerLogger = loggerFactory.CreateLogger("WebServer");
            _projectSerializer = projectSerializer;
            _webServerOptions = options.Value;
        }

        #region "Methods"

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _clientPushService.Unsubscribe(this.Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }

        public Task StartOneDas()
        {
            return Task.Run(() => _engine.Start());
        }

        public Task PauseOneDas()
        {
            return Task.Run(() => _engine.Pause());
        }

        public Task StopOneDas()
        {
            return Task.Run(() => _engine.Stop());
        }

        public Task AcknowledgeError()
        {
            return Task.Run(() => _engine.AcknowledgeError());
        }

        public Task<string> GetLastError()
        {
            return Task.Run(() => _engine.LastError);
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

                _engine.ActivateProject(projectSettings, 2);

                this.Clients.All.SendActiveProject(projectSettings);
            });
        }

        public Task<int> UpdateLiveViewSubscription(IList<Guid> channelHubGuidSet)
        {
            IList<ChannelHubBase> channelHubSettingsSet;

            return Task.Run(() =>
            {
                try
                {
                    channelHubSettingsSet = channelHubGuidSet.Select(channelHubGuid => _engine.Project.ActiveChannelHubSet.First(channelHub => channelHub.Settings.Guid == channelHubGuid)).ToList();
                }
                catch (Exception)
                {
                    throw new Exception(ErrorMessage.WebClientHub_ChannelHubNotFound);
                }

                return _clientPushService.Subscribe(this.Context.ConnectionId, channelHubSettingsSet);
            });
        }

        public Task<string> GetExtensionStringResource(string extensionId, string resourceName)
        {
            return Task.Run(() =>
            {
                return _extensionFactory.GetStringResource(extensionId, resourceName);
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
            return Task.Run(() => _extensionFactory.HandleActionRequest(actionRequest));
        }

        public Task<AppModel> GetAppModel()
        {
            string productVersion;

            productVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

            return Task.Run(async () =>
            {
                return new AppModel(
                    activeProjectSettings: _engine.Project?.Settings,
                    installedPackageSet: await _packageManager.GetInstalledPackagesAsync(),
                    clientSet: new List<string>() { },
                    dataGatewayExtensionIdentificationSet: _extensionFactory.GetIdentifications<DataGatewayExtensionSettingsBase>().ToList(),
                    dataWriterExtensionIdentificationSet: _extensionFactory.GetIdentifications<DataWriterExtensionSettingsBase>().ToList(),
                    productVersion: productVersion,
                    lastError: _engine.LastError,
                    oneDasState: _engine.OneDasState,
                    webServerOptionsLight: new WebServerOptionsLight
                    {
                        OneDasName = _webServerOptions.OneDasName,
                        AspBaseUrl = _webServerOptions.AspBaseUrl,
                        BaseDirectoryPath = _webServerOptions.BaseDirectoryPath,
                        PackageSourceSet = _packageManager.PackageSourceSet.Select(packageSource => new OneDasPackageSource(packageSource.Name, packageSource.Source)).ToList()
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
                    new List<DataGatewayExtensionSettingsBase>(),
                    new List<DataWriterExtensionSettingsBase>());
            });
        }

        public Task<DataGatewayExtensionSettingsBase> CreateDataGatewaySettings(string extensionName)
        {
            return Task.Run(() =>
            {
                return (DataGatewayExtensionSettingsBase)Activator.CreateInstance(_extensionFactory.GetSettings(extensionName));
            });
        }

        public Task<DataWriterExtensionSettingsBase> CreateDataWriterSettings(string extensionName)
        {
            return Task.Run(() =>
            {
                return (DataWriterExtensionSettingsBase)Activator.CreateInstance(_extensionFactory.GetSettings(extensionName));
            });
        }

        public Task<OneDasPackageMetaData[]> SearchPackages(string searchTerm, string address, int skip, int take)
        {
            return _packageManager.SearchAsync(searchTerm, address, skip, take);
        }

        // TODO: prevent modification of "NETStandard" package!

        public Task InstallPackage(string packageId, string source)
        {
            return Task.Run(async () =>
            {
                await _packageManager.InstallAsync(packageId, source);
                await this.UpdateClientPackagesInformation();
            });
        }

        public Task UpdatePackage(string packageId, string source)
        {
            return Task.Run(async () =>
            {
                await _packageManager.UpdateAsync(packageId, source);
                await this.UpdateClientPackagesInformation();
            });
        }

        public Task UninstallPackage(string packageId)
        {
            return Task.Run(async () =>
            {
                await _packageManager.UninstallAsync(packageId);
                await this.UpdateClientPackagesInformation();
            });
        }

        private async Task UpdateClientPackagesInformation()
        {
            await this.Clients.All.SendInstalledPackages(await _packageManager.GetInstalledPackagesAsync());

            await this.Clients.All.SendExtensionIdentifications(
                _extensionFactory.GetIdentifications<DataGatewayExtensionSettingsBase>().ToList(),
                _extensionFactory.GetIdentifications<DataWriterExtensionSettingsBase>().ToList());
        }

        #endregion
    }
}
