using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using OneDas.DataManagement.Explorer.Core;
using OneDas.DataManagement.Database;
using OneDas.Infrastructure;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using OneDas.DataManagement.Infrastructure;

namespace OneDas.DataManagement.Explorer.ViewModels
{
    public class AppStateViewModel : BindableBase, IDisposable
    {
        #region Fields

        private string _searchString;
        private string _downloadMessage;

        private double _downloadProgress;
        private double _visualizeProgress;

        private bool _isEditEnabled;
        private bool _visualizeBeginAtZero;

        private ClientState _clientState;
        private IJSRuntime _jsRuntime;

        private DataService _dataService;
        private CampaignContainer _campaignContainer;
        private OneDasDatabaseManager _databaseManager;
        private CancellationTokenSource _cts_download;
        private PropertyChangedEventHandler _propertyChanged;
        private AuthenticationStateProvider _authenticationStateProvider;

        private List<VariableInfoViewModel> _variableGroup;
        private Dictionary<string, List<DatasetInfoViewModel>> _sampleRateToSelectedDatasetsMap;
        private Dictionary<CampaignContainer, List<VariableInfoViewModel>> _campaignContainerToVariablesMap;

        #endregion

        #region Constructors

        public AppStateViewModel(IJSRuntime jsRuntime,
                                 AuthenticationStateProvider authenticationStateProvider,
                                 OneDasExplorerStateManager stateManager,
                                 OneDasDatabaseManager databaseManager,
                                 DataService dataService)
        {
            _jsRuntime = jsRuntime;
            _authenticationStateProvider = authenticationStateProvider;
            _databaseManager = databaseManager;
            _dataService = dataService;

            this.Version = Assembly.GetEntryAssembly().GetName().Version.ToString();
            this.FileGranularityValues = Utilities.GetEnumValues<FileGranularity>();
            this.FileFormatValues = Utilities.GetEnumValues<FileFormat>();
            this.NewsPaper = NewsPaper.Load();
            this.VisualizeBeginAtZero = true;

            // campaign containers and dependent init steps
            var campaignContainers = databaseManager.Database.CampaignContainers;
            var restrictedCampaigns = databaseManager.Config.RestrictedCampaigns;
            this.CampaignContainers = GetAccessibleCampainContainersAsync(campaignContainers, restrictedCampaigns).Result.AsReadOnly();

            this.SampleRateValues = this.CampaignContainers.SelectMany(campaignContainer =>
            {
                return campaignContainer.Campaign.Variables.SelectMany(variable =>
                {
                    return variable.Datasets.Select(dataset => dataset.Id.Split('_')[0]);
                });
            }).Distinct().OrderBy(x => x, new SampleRateStringComparer()).ToList();

            this.InitializeCampaignContainerToVariableMap();
            this.InitializeSampleRateToDatasetsMap();

            // state manager
            _propertyChanged = (sender, e) =>
            {
                if (e.PropertyName == nameof(OneDasExplorerStateManager.State))
                {
                    this.RaisePropertyChanged(e.PropertyName);
                }
            };

            this.StateManager = stateManager;
            this.StateManager.PropertyChanged += _propertyChanged;

            // export configuration
            this.SetExportConfiguration(new ExportConfiguration());
        }

        #endregion

        #region Properties - General

        public string Version { get; }

        public bool IsEditEnabled
        {
            get { return _isEditEnabled; }
            set
            {
#warning Make this more efficient. Maybe by tracking changes.
                if (_isEditEnabled && !value)
                {
                    _databaseManager.Database.CampaignContainers.ForEach(campaignContainer =>
                    {
                        _databaseManager.SaveCampaignMeta(campaignContainer.CampaignMeta);
                    });
                }

                this.SetProperty(ref _isEditEnabled, value);
            }
        }

        public ClientState ClientState
        {
            get { return _clientState; }
            set { this.SetProperty(ref _clientState, value); }
        }

        public double DownloadProgress
        {
            get { return _downloadProgress; }
            set { this.SetProperty(ref _downloadProgress, value); }
        }

        public string DownloadMessage
        {
            get { return _downloadMessage; }
            set { this.SetProperty(ref _downloadMessage, value); }
        }

        internal OneDasExplorerStateManager StateManager { get; }

        internal ExportConfiguration ExportConfiguration { get; private set; }

        #endregion

        #region Properties - Settings

        // this is required because MatBlazor converts dates to local representation which is not desired
        public DateTime DateTimeBeginWorkaround
        {
            get { return DateTime.SpecifyKind(this.DateTimeBegin, DateTimeKind.Local); }
            set 
            { 
                if (value.Kind == DateTimeKind.Local)
                    this.DateTimeBegin = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                else
                    this.DateTimeBegin = DateTime.SpecifyKind(TimeZoneInfo.ConvertTimeFromUtc(value, TimeZoneInfo.Local), DateTimeKind.Utc);
            }
        }

        public DateTime DateTimeEndWorkaround
        {
            get { return DateTime.SpecifyKind(this.DateTimeEnd, DateTimeKind.Local); }
            set
            {
                if (value.Kind == DateTimeKind.Local)
                    this.DateTimeEnd = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                else
                    this.DateTimeEnd = DateTime.SpecifyKind(TimeZoneInfo.ConvertTimeFromUtc(value, TimeZoneInfo.Local), DateTimeKind.Utc);
            }
        }

        public DateTime DateTimeBeginMaximum { get; private set; }

        public DateTime DateTimeEndMinimum { get; private set; }

        public List<string> SampleRateValues { get; set; }

        public List<FileGranularity> FileGranularityValues { get; }

        public List<FileFormat> FileFormatValues { get; }

        public bool VisualizeBeginAtZero
        {
            get { return _visualizeBeginAtZero; }
            set { this.SetProperty(ref _visualizeBeginAtZero, value); }
        }

        #endregion

        #region Properties - Channel Selection

        public CampaignContainer CampaignContainer
        {
            get
            {
                return _campaignContainer;
            }
            set
            {
                this.SetProperty(ref _campaignContainer, value);

                _searchString = string.Empty;

                this.UpdateGroupedVariables();
                this.UpdateAttachments();
            }
        }

        public List<string> Attachments { get; private set; }

        public ReadOnlyCollection<CampaignContainer> CampaignContainers { get; }

        public Dictionary<string, List<VariableInfoViewModel>> GroupedVariables { get; private set; }

        public List<VariableInfoViewModel> VariableGroup
        {
            get { return _variableGroup; }
            set { base.SetProperty(ref _variableGroup, value); }
        }

        public string SearchString
        {
            get { return _searchString; }
            set
            {
                base.SetProperty(ref _searchString, value);
                this.UpdateGroupedVariables();
            }
        }

        #endregion

        #region Properties - News

        public NewsPaper NewsPaper { get; }

        #endregion

        #region Properties - Download Area

        public IReadOnlyCollection<DatasetInfoViewModel> SelectedDatasets => this.GetSelectedDatasets();

        #endregion

        #region Properties - Visualization

        public double VisualizeProgress
        {
            get { return _visualizeProgress; }
            set { this.SetProperty(ref _visualizeProgress, value); }
        }

        #endregion

        #region Properties - Relay Properties

        public OneDasExplorerState State => this.StateManager.State;

        public DateTime DateTimeBegin
        {
            get { return this.ExportConfiguration.DateTimeBegin; }
            set
            {
                this.ExportConfiguration.DateTimeBegin = value;
                this.RaisePropertyChanged();

                this.DateTimeEndMinimum = value;
            }
        }

        public DateTime DateTimeEnd
        {
            get { return this.ExportConfiguration.DateTimeEnd; }
            set
            {
                this.ExportConfiguration.DateTimeEnd = value;
                this.RaisePropertyChanged();

                this.DateTimeBeginMaximum = value;
            }
        }

        public FileGranularity FileGranularity
        {
            get { return this.ExportConfiguration.FileGranularity; }
            set 
            {
                this.ExportConfiguration.FileGranularity = value;
                this.RaisePropertyChanged();
            }
        }

        public FileFormat FileFormat
        {
            get { return this.ExportConfiguration.FileFormat; }
            set
            {
                this.ExportConfiguration.FileFormat = value;
                this.RaisePropertyChanged();
            }
        }

        public string SampleRate
        {
            get { return this.ExportConfiguration.SampleRate; }
            set
            {
                this.ExportConfiguration.SampleRate = value;
                this.UpdateExportConfiguration();
                this.RaisePropertyChanged();

                this.RaisePropertyChanged(nameof(AppStateViewModel.SelectedDatasets));
            }
        }

        #endregion

        #region Methods

        public List<string> GetPresets()
        {
            var folderPath = "PRESETS";
            return Directory.EnumerateFiles(folderPath, "*.json", SearchOption.TopDirectoryOnly).ToList();
        }

        public bool CanDownload()
        {
            return this.DateTimeBegin < this.DateTimeEnd &&
                   this.SelectedDatasets.Count > 0 &&
                   (ulong)this.FileGranularity >= 86400 / new SampleRateContainer(this.SampleRate).SamplesPerDay &&
                   this.State == OneDasExplorerState.Ready &&
                   !this.SampleRate.Contains("600 s");
        }

        public async Task DownloadAsync(IPAddress remoteIpAdress)
        {
            _cts_download = new CancellationTokenSource();

            EventHandler<ProgressUpdatedEventArgs> eventHandler = (sender, e) =>
            {
                this.DownloadMessage = e.Message;
                this.DownloadProgress = e.Progress;
            };

            try
            {
                this.ClientState = ClientState.PrepareDownload;
                _dataService.Progress.ProgressChanged += eventHandler;

                var sampleRateContainer = new SampleRateContainer(this.SampleRate);
                var selectedDatasets = this.GetSelectedDatasets().Select(dataset => dataset.Model).ToList();

                var downloadLink = await _dataService.ExportDataAsync(remoteIpAdress,
                                                                       this.DateTimeBegin,
                                                                       this.DateTimeEnd,
                                                                       sampleRateContainer,
                                                                       this.FileFormat,
                                                                       this.FileGranularity,
                                                                       selectedDatasets,
                                                                       _cts_download.Token);

                if (!string.IsNullOrWhiteSpace(downloadLink))
                {
                    var fileName = downloadLink.Split("/").Last();
                    await JsInterop.FileSaveAs(_jsRuntime, fileName, downloadLink);
                }
            }
            finally
            {
                _dataService.Progress.ProgressChanged -= eventHandler;
                this.ClientState = ClientState.Normal;
                this.DownloadMessage = string.Empty;
                this.DownloadProgress = 0;
            }
        }

        public void CancelDownload()
        {
            _cts_download.Cancel();
        }

        public void ToggleDataAvailability()
        {
            if (this.ClientState != ClientState.DataAvailability)
                this.ClientState = ClientState.DataAvailability;
            else
                this.ClientState = ClientState.Normal;
        }

        public bool CanVisualize()
        {
            return this.SelectedDatasets.Any()
                && this.DateTimeBegin < this.DateTimeEnd;
        }

        public bool IsSizeLimitExceeded()
        {
            return this.GetByteCount() > 20 * 1000 * 1000;
        }

        public void ToggleDataVisualization()
        {
            if (this.ClientState != ClientState.DataVisualizing)
                this.ClientState = ClientState.DataVisualizing;
            else
                this.ClientState = ClientState.Normal;
        }

        [JSInvokable]
        public void SetVisualizeProgress(double progress)
        {
            this.VisualizeProgress = progress;
        }

        public async Task<DataAvailabilityStatistics> GetDataAvailabilityStatisticsAsync()
        {
            return await _dataService.GetDataAvailabilityStatisticsAsync(this.CampaignContainer.Name, this.DateTimeBegin, this.DateTimeEnd);
        }

        public void SetExportConfiguration(ExportConfiguration exportConfiguration)
        {
            this.DateTimeBeginMaximum = exportConfiguration.DateTimeEnd;
            this.DateTimeEndMinimum = exportConfiguration.DateTimeBegin;
            this.InitializeSampleRateToDatasetsMap();

            this.ExportConfiguration = exportConfiguration;
            var selectedDatasets = this.GetSelectedDatasets();

            exportConfiguration.Variables.ForEach(value =>
            {
                var pathParts = value.Split('/');
                var campaignName = $"/{pathParts[1]}/{pathParts[2]}/{pathParts[3]}";
                var variableName = pathParts[4];
                var datasetName = pathParts[5];

                var campaignContainer = this.CampaignContainers.FirstOrDefault(current => current.Name == campaignName);

                if (campaignContainer != null)
                {
                    var variables = _campaignContainerToVariablesMap[campaignContainer];
                    var variable = variables.FirstOrDefault(current => current.Id == variableName);

                    if (variable != null)
                    {
                        var dataset = variable.Datasets.FirstOrDefault(current => current.Name == datasetName);

                        if (dataset != null)
                            selectedDatasets.Add(dataset);
                    }
                }
            });


            this.RaisePropertyChanged(nameof(AppStateViewModel.ExportConfiguration));
        }

        public bool IsDatasetSeleced(DatasetInfoViewModel dataset)
        {
            return this.SelectedDatasets.Contains(dataset);
        }

        public void ToggleDatasetSelection(DatasetInfoViewModel dataset)
        {
            var isSelected = this.SelectedDatasets.Contains(dataset);

            if (isSelected)
                this.GetSelectedDatasets().Remove(dataset);
            else
                this.GetSelectedDatasets().Add(dataset);

            this.UpdateExportConfiguration();
            this.RaisePropertyChanged(nameof(AppStateViewModel.SelectedDatasets));
        }

        public long GetByteCount()
        {
            var totalDays = (this.DateTimeEnd - this.DateTimeBegin).TotalDays;
            var frequency = string.IsNullOrWhiteSpace(this.SampleRate) ? 0 : new SampleRateContainer(this.SampleRate).SamplesPerDay;

            return (long)this.GetSelectedDatasets().Sum(dataset =>
            {
                var elementSize = OneDasUtilities.SizeOf(dataset.DataType);

                return frequency * totalDays * elementSize;
            });
        }

        private void UpdateAttachments()
        {
            this.Attachments = null;

            if (this.CampaignContainer != null)
            {
                var folderPath = Path.Combine(Environment.CurrentDirectory, "ATTACHMENTS", this.CampaignContainer.PhysicalName);

                if (Directory.Exists(folderPath))
                    this.Attachments = Directory.GetFiles(folderPath, "*").ToList();
            }
        }

        private void UpdateGroupedVariables()
        {
            if (this.CampaignContainer != null)
            {
                this.GroupedVariables = new Dictionary<string, List<VariableInfoViewModel>>();

                foreach (var variable in _campaignContainerToVariablesMap[this.CampaignContainer])
                {
                    if (this.VariableMatchesFilter(variable))
                    {
                        var groupNames = variable.Group.Split('\n');

                        foreach (string groupName in groupNames)
                        {
                            var success = this.GroupedVariables.TryGetValue(groupName, out var group);

                            if (!success)
                            {
                                group = new List<VariableInfoViewModel>();
                                this.GroupedVariables[groupName] = group;
                            }

                            group.Add(variable);
                        }
                    }
                }

                foreach (var entry in this.GroupedVariables)
                {
                    entry.Value.Sort((x, y) => x.Name.CompareTo(y.Name));
                }
            }

            if (this.GroupedVariables.Any())
                this.VariableGroup = this.GroupedVariables.First().Value;
            else
                this.VariableGroup = null;
        }

        private void UpdateExportConfiguration()
        {
            this.ExportConfiguration.Variables = this.GetSelectedDatasets().Select(dataset =>
            {
                return $"{dataset.Parent.Parent.Id}/{dataset.Parent.Id}/{dataset.Name}";
            }).ToList();
        }

        private bool VariableMatchesFilter(VariableInfoViewModel variable)
        {
            if (string.IsNullOrWhiteSpace(this.SearchString))
                return true;

            if (variable.Name.Contains(this.SearchString, StringComparison.OrdinalIgnoreCase) 
             || variable.Description.Contains(this.SearchString, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        private void InitializeCampaignContainerToVariableMap()
        {
            _campaignContainerToVariablesMap = new Dictionary<CampaignContainer, List<VariableInfoViewModel>>();

            foreach (var campaignContainer in this.CampaignContainers)
            {
                _campaignContainerToVariablesMap[campaignContainer] = campaignContainer.Campaign.Variables.Select(variable =>
                {
                    var variableMeta = campaignContainer.CampaignMeta.Variables.FirstOrDefault(variableMeta => variableMeta.Id == variable.Id);
                    return new VariableInfoViewModel(variable, variableMeta);
                }).ToList();
            }
        }

        private void InitializeSampleRateToDatasetsMap()
        {
            _sampleRateToSelectedDatasetsMap = this.SampleRateValues.ToDictionary(sampleRate => sampleRate, sampleRate => new List<DatasetInfoViewModel>());
        }

        private List<DatasetInfoViewModel> GetSelectedDatasets()
        {
            var containsKey = !string.IsNullOrWhiteSpace(this.SampleRate) && _sampleRateToSelectedDatasetsMap.ContainsKey(this.SampleRate);

            if (containsKey)
                return _sampleRateToSelectedDatasetsMap[this.SampleRate];
            else
                return new List<DatasetInfoViewModel>();
        }

        private async Task<List<CampaignContainer>> GetAccessibleCampainContainersAsync(List<CampaignContainer> campaignContainers, List<string> restrictedCampaigns)
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var principal = authState.User;
            var visibleCampaigns = new List<CampaignContainer>();

            return campaignContainers.Where(campaignContainer =>
            {
                return Utilities.IsCampaignAccessible(principal, campaignContainer.Campaign, restrictedCampaigns)
                    && Utilities.IsCampaignVisible(principal, campaignContainer.Campaign, new List<string>() { "/IN_MEMORY/ALLOWED/TEST", "/IN_MEMORY/RESTRICTED/TEST" });
            }).ToList();
        }

        #endregion

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _cts_download?.Cancel();
                    this.StateManager.PropertyChanged -= _propertyChanged;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
