using Microsoft.AspNetCore.SignalR;
using OneDas.Common;
using OneDas.Engine.Core;
using OneDas.Engine.Serialization;
using OneDas.Infrastructure;
using OneDas.Plugin;
using OneDas.WebServer.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace OneDas.WebServer.Web
{
    public class WebClientHub : Hub<IWebClientHub>
    {
        private PluginManager _pluginManager;
        private OneDasEngine _oneDasEngine;

        public WebClientHub(PluginManager pluginManager, OneDasEngine oneDasEngine)
        {
            _pluginManager = pluginManager;
            _oneDasEngine = oneDasEngine;
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

        public void StartOneDas()
        {
            _oneDasEngine.OneDasState = OneDasState.Run;
        }

        public void StopOneDas()
        {
            _oneDasEngine.OneDasState = OneDasState.Ready;
        }

        public void AcknowledgeError()
        {
            _oneDasEngine.OneDasState = OneDasState.Initialization;
            _oneDasEngine.OneDasState = OneDasState.Unconfigured;
        }

        public Task<string> GetLastError()
        {
            return Task.Run(() => _oneDasEngine.LastError);
        }

        public Task SaveSlimOneDasSettings(SlimOneDasSettings slimOneDasSettings)
        {
            return Task.Run(() =>
            {
                ConfigurationManager<WebServerOptions>.Options.OneDasName = slimOneDasSettings.OneDasName;
                ConfigurationManager<WebServerOptions>.Options.AspBaseUrl = slimOneDasSettings.AspBaseUrl;
                ConfigurationManager<WebServerOptions>.Options.NewBaseDirectoryPath = slimOneDasSettings.BaseDirectoryPath;
                ConfigurationManager<WebServerOptions>.Save();

                this.Clients.All.SendSlimOneDasSettings(slimOneDasSettings);
            });
        }

        public Task SaveProject(Project project)
        {
            string fileName;
            string directoryPath;
            string currentFilePath;
            string newFilePath;

            Contract.Requires(project != null);

            project.Validate();

            directoryPath = Path.Combine(ConfigurationManager<WebServerOptions>.Options.BaseDirectoryPath, "project");
            fileName = $"{project.Description.CampaignPrimaryGroup}_{project.Description.CampaignSecondaryGroup}_{project.Description.CampaignName}_{project.Description.Guid.ToString()}.json";
            currentFilePath = Path.Combine(directoryPath, fileName);

            return Task.Run(() =>
            {
                try
                {
                    if (File.Exists(currentFilePath))
                    {
                        newFilePath = Path.Combine(ConfigurationManager<WebServerOptions>.Options.BaseDirectoryPath, "backup", $"{ DateTime.UtcNow.ToString("yyyy-MM-ddTHH-mm-ss") }_{ fileName }");

                        File.Copy(currentFilePath, newFilePath, true);
                    }
                }
                finally
                {
                    currentFilePath = project.Save(currentFilePath);

                    //Bootloader.WebServerLogger.LogInformation("project file saved");
                }
            });
        }

        public Task ActivateProject(ProjectDescription projectDescription)
        {
            return Task.Run(() =>
            {
                string filePath;
                Project project;

                projectDescription.Validate();

                // Improve: Make more flexible, renaming of file is impossible like that
                filePath = Path.Combine(ConfigurationManager<WebServerOptions>.Options.BaseDirectoryPath, "project", $"{projectDescription.CampaignPrimaryGroup}_{projectDescription.CampaignSecondaryGroup}_{projectDescription.CampaignName}_{projectDescription.Guid}.json");
                project = ProjectSerializationHelper.Load(filePath);

                ConfigurationManager<WebServerOptions>.Options.CurrentProjectFilePath = filePath;
                ConfigurationManager<WebServerOptions>.Save();

                _oneDasEngine.ActivateProject(project, 2);

                this.Clients.All.SendActiveProject(project);
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
                    throw new Exception(ErrorMessage.Broadcaster_ChannelHubNotFound);
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
                List<Type> typeSet;
                Type type;
                Assembly assembly;

                typeSet = _pluginManager.GetPluginsByBaseClass<PluginSettingsBase>().ToList();
                type = typeSet.FirstOrDefault(x => x.GetFirstAttribute<PluginIdentificationAttribute>().Id == pluginId);
                assembly = type?.Assembly;
                resourceName = $"{ assembly.GetName().Name }.{ resourceName }";

                if (assembly != null)
                {
                    using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
                    {
                        if (resourceStream != null)
                        {
                            using (StreamReader reader = new StreamReader(resourceStream))
                            {
                                return reader.ReadToEnd();
                            }
                        }
                        else
                        {
                            throw new BroadcasterException($"The requested resource of plugin ID = '{ pluginId }' and name = '{ resourceName }' could not be found.");
                        }
                    }
                }
                else
                {
                    throw new BroadcasterException($"The requested plugin with ID = '{ pluginId }' could not be found.");
                }
            });
        }

        public Task<string> GetPluginFile(string pluginType, string relativePath)
        {
            return Task.Run(() =>
            {
                try
                {
                    string basePath = Path.Combine(ConfigurationManager<WebServerOptions>.Options.BaseDirectoryPath, "plugin");
                    string filePath = Path.GetFullPath(Path.Combine(basePath, pluginType, relativePath));

                    if (filePath.StartsWith(Path.Combine(ConfigurationManager<WebServerOptions>.Options.BaseDirectoryPath, "plugin")))
                    {
                        if (File.Exists(filePath))
                        {
                            return File.ReadAllText(filePath);
                        }
                        else
                        {
                            throw new BroadcasterException($"The requested file with type = '{ pluginType }' and path = '{ relativePath }' could not be found.");
                        }
                    }
                    else
                    {
                        throw new BroadcasterException($"The requested path '{ relativePath }' does not point into the plugin folder.");
                    }

                }
                catch (Exception ex)
                {
                    if (ex is BroadcasterException)
                    {
                        throw;
                    }
                    else
                    {
                        throw new Exception($"An unknown error occured.");
                    }
                }
            });
        }

        public Task<IEnumerable<ProjectDescription>> GetProjectDescriptions()
        {
            return Task.Run(() =>
            {
                IEnumerable<string> filePathSet;
                IList<ProjectDescription> projectDescriptionSet;

                filePathSet = Directory.GetFiles(Path.Combine(ConfigurationManager<WebServerOptions>.Options.BaseDirectoryPath, "project"), "*.json");
                projectDescriptionSet = new List<ProjectDescription>();

                foreach (string filePath in filePathSet)
                {
                    try
                    {
                        projectDescriptionSet.Add(SerializationHelper.GetProjectDescriptonFromFile(filePath));
                    }
                    catch (Exception)
                    {
                        //
                    }
                }

                return (IEnumerable<ProjectDescription>)projectDescriptionSet;
            });
        }

        public Task<Project> OpenProject(ProjectDescription projectDescription)
        {
            return Task.Run(() =>
            {
                // Improve: Make more flexible, renaming of file is impossible like that
                Project project = ProjectSerializationHelper.Load(Path.Combine(ConfigurationManager<WebServerOptions>.Options.BaseDirectoryPath, "project", $"{projectDescription.CampaignPrimaryGroup}_{projectDescription.CampaignSecondaryGroup}_{projectDescription.CampaignName}_{projectDescription.Guid}.json"));

                return project;
            });
        }

        public Task<ActionResponse> RequestAction(ActionRequest actionRequest)
        {
            return Task.Run(() =>
            {
                List<Type> typeSet;

                typeSet = _pluginManager.GetPluginsByBaseClass<PluginSettingsBase>().ToList();
                actionRequest.Validate();

                if (actionRequest.InstanceId > 0)
                {
                    throw new NotImplementedException();
                }
                else // call static handler
                {
                    Type pluginSettingsType;
                    PluginActionRequestAttribute attribute;

                    pluginSettingsType = typeSet.FirstOrDefault(x => x.GetFirstAttribute<PluginIdentificationAttribute>().Id == actionRequest.PluginId);

                    if (pluginSettingsType != null)
                    {
                        try
                        {
                            attribute = pluginSettingsType.GetFirstAttribute<PluginActionRequestAttribute>();

                            if (attribute != null)
                            {
                                MethodInfo actionRequestHandler = attribute.Type.GetMethod(attribute.MethodName, BindingFlags.Public | BindingFlags.Static);

                                return (ActionResponse)actionRequestHandler.Invoke(null, new object[] { actionRequest, (Func<object, Type, object>)SerializationHelper.Deserialize });
                            }
                            else
                            {
                                throw new Exception(ErrorMessage.Broadcaster_MissingPluginActionRequestAttribute);
                            }
                        }
                        catch (TargetInvocationException ex)
                        {
                            throw ex.InnerException;
                        }
                    }
                    else
                    {
                        throw new Exception(ErrorMessage.Broadcaster_PluginNotFound);
                    }
                }
            });
        }

        public Task<AppModel> GetAppModel()
        {
            IList<PluginIdentificationAttribute> dataGatewayPluginIdentificationSet;
            IList<PluginIdentificationAttribute> dataWriterPluginIdentificationSet;

            dataGatewayPluginIdentificationSet = _pluginManager.Get<DataGatewayPluginSettingsBase>().Select(dataGatewaySettingsType =>
            {
                PluginIdentificationAttribute pluginIdentificationAttribute;

                pluginIdentificationAttribute = dataGatewaySettingsType.GetFirstAttribute<PluginIdentificationAttribute>();
                pluginIdentificationAttribute.ProductVersion = FileVersionInfo.GetVersionInfo(dataGatewaySettingsType.Assembly.Location).ProductVersion;

                return pluginIdentificationAttribute;
            }).ToList();

            dataWriterPluginIdentificationSet = _pluginManager.Get<DataWriterPluginSettingsBase>().Select(dataWriterSettingsType =>
            {
                PluginIdentificationAttribute pluginIdentificationAttribute;

                pluginIdentificationAttribute = dataWriterSettingsType.GetFirstAttribute<PluginIdentificationAttribute>();
                pluginIdentificationAttribute.ProductVersion = FileVersionInfo.GetVersionInfo(dataWriterSettingsType.Assembly.Location).ProductVersion;

                return pluginIdentificationAttribute;
            }).ToList();

            return Task.Run(() =>
            {
                return new AppModel(
                    activeProject: _oneDasEngine.Project,
                    clientSet: new List<string>() { "Horst", "Köhler" },
                    dataGatewayPluginIdentificationSet: dataGatewayPluginIdentificationSet,
                    dataWriterPluginIdentificationSet: dataWriterPluginIdentificationSet,
                    lastError: _oneDasEngine.LastError,
                    oneDasState: _oneDasEngine.OneDasState,
                    slimOneDasSettings: new SlimOneDasSettings
                    {
                        OneDasName = ConfigurationManager<WebServerOptions>.Options.OneDasName,
                        AspBaseUrl = ConfigurationManager<WebServerOptions>.Options.AspBaseUrl,
                        BaseDirectoryPath = ConfigurationManager<WebServerOptions>.Options.BaseDirectoryPath
                    });
            });
        }

        public Task<Project> CreateProject(string campaignPrimaryGroup, string campaignSecondaryGroup, string configurationName)
        {
            return Task.Run(() =>
            {
                IList<DataGatewayPluginSettingsBase> dataGatewayPluginSettingsSet;
                IList<DataWriterPluginSettingsBase> dataWriterPluginSettingsSet;

                dataGatewayPluginSettingsSet = _pluginManager.Get<DataGatewayPluginSettingsBase>().
                        Where(pluginSettingsType => pluginSettingsType.GetFirstAttribute<PluginIdentificationAttribute>().Id == "EtherCAT").
                        Select(pluginSettingsType => _pluginManager.BuildSettings<DataGatewayPluginSettingsBase>(pluginSettingsType)).ToList();

                dataWriterPluginSettingsSet = _pluginManager.Get<DataWriterPluginSettingsBase>().
                        Where(pluginSettingsType => pluginSettingsType.GetFirstAttribute<PluginIdentificationAttribute>().Id == "HDF").
                        Select(pluginSettingsType => _pluginManager.BuildSettings<DataWriterPluginSettingsBase>(pluginSettingsType)).ToList();

                return new Project(
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
                return _pluginManager.BuildSettings<DataGatewayPluginSettingsBase>(pluginName);
            });
        }

        public Task<DataWriterPluginSettingsBase> CreateDataWriterSettings(string pluginName)
        {
            return Task.Run(() =>
            {
                return _pluginManager.BuildSettings<DataWriterPluginSettingsBase>(pluginName);
            });
        }
    }
}
