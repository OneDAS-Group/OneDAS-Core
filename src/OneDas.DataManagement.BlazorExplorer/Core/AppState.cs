using OneDas.DataManagement.Database;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OneDas.DataManagement.BlazorExplorer.Core
{
    public class AppState
    {
        #region Fields

        private DateTime _dateTimeBegin;
        private DateTime _dateTimeEnd;
        private CampaignContainer _campaignContainer;

        #endregion

        #region Constructors

        public AppState()
        {
            this.Version = Assembly.GetEntryAssembly().GetName().Version.ToString();

            this.DateTimeBegin = DateTime.UtcNow.Date.AddDays(-2);
            this.DateTimeEnd = DateTime.UtcNow.Date.AddDays(-1);
            this.DateTimeBeginMaximum = this.DateTimeEnd;
            this.DateTimeEndMinimum = this.DateTimeBegin;

            this.SampleRateValues = new List<string>() { "60 s", "1 s", "100 Hz", "250 Hz", "2500 Hz" };

            this.FileGranularity = FileGranularity.Hour;
            this.FileGranularityValues = Utilities.GetEnumValues<FileGranularity>();

            this.FileFormat = FileFormat.CSV;
            this.FileFormatValues = Utilities.GetEnumValues<FileFormat>();

            this.CampaignContainers = Program.DatabaseManager.Database.CampaignContainers.AsReadOnly();

            this.NewsPaper = NewsPaper.Load();
        }

        #endregion

        #region Properties

        public string Version { get; }
        
        public DateTime DateTimeBegin
        {
            get
            {
                return _dateTimeBegin;
            }
            set
            {
                _dateTimeBegin = value;
                this.DateTimeEndMinimum = value;
            }
        }

        public DateTime DateTimeBeginMaximum;

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
                _campaignContainer = value;
                this.UpdateGroupedVariables();
                this.UpdateAttachments();
            }
        }

        public List<string> Attachments { get; private set; }

        public ReadOnlyCollection<CampaignContainer> CampaignContainers { get; }

        public Dictionary<string, List<VariableInfoViewModel>> GroupedVariables { get; private set; }

        public List<VariableInfoViewModel> VariableGroup { get; set; }

        public NewsPaper NewsPaper { get; }

        public string SearchString { get; set; }

        #endregion

        #region Serializable

        public DateTime DateTimeEnd
        {
            get
            {
                return _dateTimeEnd;
            }
            set
            {
                _dateTimeEnd = value;
                this.DateTimeBeginMaximum = value;
            }
        }

        public DateTime DateTimeEndMinimum;

        public string SampleRate { get; set; }

        public FileGranularity FileGranularity { get; set; }

        public FileFormat FileFormat { get; set; }

        #endregion

        #region Methods

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

                foreach (var variable in this.CampaignContainer.Campaign.Variables)
                {
                    var variableMeta = this.CampaignContainer.CampaignMeta.Variables.FirstOrDefault(variableMeta => variableMeta.Name == variable.Name);
                    var variableViewModel = new VariableInfoViewModel(variable, variableMeta);

                    var groupNames = variable.VariableGroups.Last().Split('\n');

                    foreach (string groupName in groupNames)
                    {
                        var success = this.GroupedVariables.TryGetValue(groupName, out var group);
                        
                        if (!success)
                        {
                            group = new List<VariableInfoViewModel>();
                            this.GroupedVariables[groupName] = group;
                        }

                        group.Add(variableViewModel);
                    }
                }

                foreach (var entry in this.GroupedVariables)
                {
                    entry.Value.Sort((x, y) => x.Name.CompareTo(y.Name));
                }
            }
        }

        #endregion
    }
}
