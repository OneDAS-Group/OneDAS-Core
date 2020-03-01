using OneDas.DataManagement.Database;
using OneDas.Infrastructure;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OneDas.DataManagement.BlazorExplorer.Core
{
    public class AppStateViewModel : BindableBase
    {
        #region Fields

#warning make this private!
        public ExportConfiguration _model;

        private string _searchString;

        private CampaignContainer _campaignContainer;
        private List<VariableInfoViewModel> _variableGroup;

        private Dictionary<string, List<DatasetInfoViewModel>> _sampleRateToSelectedDatasetsMap;
        private Dictionary<CampaignContainer, List<VariableInfoViewModel>> _campaignContainerToVariablesMap;

        #endregion

        #region Constructors

        public AppStateViewModel() : this(new ExportConfiguration())
        {
            //
        }

        public AppStateViewModel(ExportConfiguration exportConfiguration)
        {
            this.SetExportConfiguration(exportConfiguration);

            this.Version = Assembly.GetEntryAssembly().GetName().Version.ToString();
            this.FileGranularityValues = Utilities.GetEnumValues<FileGranularity>();
            this.FileFormatValues = Utilities.GetEnumValues<FileFormat>();

            this.CampaignContainers = Program.DatabaseManager.Database.CampaignContainers.AsReadOnly();

            this.SampleRateValues = this.CampaignContainers.SelectMany(campaignContainer =>
            {
                return campaignContainer.Campaign.Variables.SelectMany(variable =>
                {
                    return variable.Datasets.Select(dataset => dataset.Name.Split('_')[0])
                                            .Where(sampleRate => !sampleRate.Contains("600 s"));
                });
            }).Distinct().OrderBy(x => x, new SampleRateStringComparer()).ToList();

            this.NewsPaper = NewsPaper.Load();

            this.InitializeCampaignContainerToVariableMap();
            this.InitializeSampleRateToDatasetsMap();
        }

        #endregion

        #region Properties

        public string Version { get; }
        
        public DateTime DateTimeBeginMaximum { get; private set; }

        public DateTime DateTimeEndMinimum { get; private set; }

        public List<string> SampleRateValues { get; set; }

        public List<FileGranularity> FileGranularityValues { get; }

        public List<FileFormat> FileFormatValues { get; }

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

        public NewsPaper NewsPaper { get; }

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

        #region Relay Properties

        public DateTime DateTimeBegin
        {
            get { return _model.DateTimeBegin; }
            set
            {
                base.SetProperty(ref _model.DateTimeBeginField, value);
                this.DateTimeEndMinimum = value;
            }
        }

        public DateTime DateTimeEnd
        {
            get { return _model.DateTimeEnd; }
            set
            {
                base.SetProperty(ref _model.DateTimeEndField, value);
                this.DateTimeBeginMaximum = value;
            }
        }

        public FileGranularity FileGranularity
        {
            get { return _model.FileGranularity; }
            set { base.SetProperty(ref _model.FileGranularityField, value); }
        }

        public FileFormat FileFormat
        {
            get { return _model.FileFormat; }
            set { base.SetProperty(ref _model.FileFormatField, value); }
        }

        public string SampleRate
        {
            get { return _model.SampleRate; }
            set
            {
                base.SetProperty(ref _model.SampleRateField, value);
                this.RaisePropertyChanged(nameof(AppStateViewModel.SelectedDatasets));
            }
        }

        public IReadOnlyCollection<DatasetInfoViewModel> SelectedDatasets => this.GetSelectedDatasets();

        #endregion

        #region Methods

        public void SetExportConfiguration(ExportConfiguration exportConfiguration)
        {
            _model = exportConfiguration;

            this.DateTimeBeginMaximum = this.DateTimeEnd;
            this.DateTimeEndMinimum = this.DateTimeBegin;

            this.RaisePropertyChanged(nameof(AppStateViewModel));
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

            this.RaisePropertyChanged(nameof(AppStateViewModel.SelectedDatasets));
        }

        private void UpdateAttachments()
        {
            this.Attachments = null;

            if (this.CampaignContainer != null)
            {
                var folderPath = Path.Combine(Environment.CurrentDirectory, "META", this.CampaignContainer.PhysicalName);

                if (Directory.Exists(folderPath))
                    this.Attachments = Directory.GetFiles(folderPath, "*").ToList();
            }
        }

        private void UpdateGroupedVariables()
        {
            this.VariableGroup = null;

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
                    var variableMeta = campaignContainer.CampaignMeta.Variables.FirstOrDefault(variableMeta => variableMeta.Name == variable.Name);
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
            if (string.IsNullOrWhiteSpace(this.SampleRate))
                return new List<DatasetInfoViewModel>();
            else
                return _sampleRateToSelectedDatasetsMap[this.SampleRate];
        }

        #endregion
    }
}
