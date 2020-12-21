using Microsoft.Extensions.Logging;
using OneDas.DataManagement.Database;
using OneDas.DataManagement.Explorer.ViewModels;
using OneDas.DataManagement.Infrastructure;
using OneDas.Extension.Csv;
using OneDas.Infrastructure;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OneDas.DataManagement.Explorer.Core
{
    public class AppState : BindableBase, IDisposable
    {
        #region Events

        public event EventHandler<OneDasDatabase> DatabaseUpdated;

        #endregion

        #region Fields

        private OneDasDatabaseManager _databaseManager;
        private Dictionary<ProjectContainer, List<ChannelInfoViewModel>> _channelCache;

        #endregion

        #region Constructors

        public AppState(ILogger<AppState> logger,
                        OneDasDatabaseManager databaseManager,
                        OneDasExplorerOptions options)
        {
            this.Logger = logger;
            _databaseManager = databaseManager;
            _databaseManager.PropertyChanged += this.OnDatabasManagerPropertyChanged;
            _databaseManager.DatabaseUpdated += this.OnDatabaseUpdated;

            this.Version = Assembly.GetEntryAssembly().GetName().Version.ToString();
            this.FileGranularityValues = Utilities.GetEnumValues<FileGranularity>();
            this.FileFormatValues = Utilities.GetEnumValues<FileFormat>();
            this.CodeTypeValues = Utilities.GetEnumValues<CodeType>();
            this.CodeLanguageValues = Utilities.GetEnumValues<CodeLanguage>();
            this.CsvRowIndexFormatValues = Utilities.GetEnumValues<CsvRowIndexFormat>();
            this.NewsPaper = NewsPaper.Load(Path.Combine(options.DataBaseFolderPath, "news.json"));

            _channelCache = new Dictionary<ProjectContainer, List<ChannelInfoViewModel>>();

            // load filter settings
            var filterSettingsFilePath = Path.Combine(options.DataBaseFolderPath, "filters.json");
            this.FilterSettings = new FilterSettingsViewModel(filterSettingsFilePath);
        }

        #endregion

        #region Properties - General

        public bool IsDatabaseUpdating => _databaseManager.IsUpdating;

        public ILogger<AppState> Logger { get; }

        public string Version { get; }

        #endregion

        #region Properties - Settings

        public List<FileGranularity> FileGranularityValues { get; }

        public List<FileFormat> FileFormatValues { get; }

        public List<CsvRowIndexFormat> CsvRowIndexFormatValues { get; }

        #endregion

        #region Properties - Filter

        public List<CodeType> CodeTypeValues { get; set; }

        public List<CodeLanguage> CodeLanguageValues { get; set; }

        public FilterSettingsViewModel FilterSettings { get; }

        #endregion

        #region Properties - News

        public NewsPaper NewsPaper { get; }

        #endregion

        #region Methods

        public void ClearCache()
        {
            _channelCache.Clear();
        }

        public List<ChannelInfoViewModel> GetChannels(ProjectContainer projectContainer)
        {
            if (!_channelCache.TryGetValue(projectContainer, out var channels))
            {
                channels = projectContainer.Project.Channels.Select(channel =>
                {
                    var channelMeta = projectContainer.ProjectMeta.Channels.First(channelMeta => channelMeta.Id == channel.Id);
                    return new ChannelInfoViewModel(channel, channelMeta);
                }).ToList();

                _channelCache[projectContainer] = channels;
            }

            return channels;
        }

        #endregion

        #region Callbacks

        private void OnDatabaseUpdated(object sender, OneDasDatabase e)
        {
            this.ClearCache();
            this.DatabaseUpdated?.Invoke(this, e);
        }

        private void OnDatabasManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(OneDasDatabaseManager.IsUpdating))
            {
                this.RaisePropertyChanged(nameof(AppState.IsDatabaseUpdating));
            }
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
                    _databaseManager.DatabaseUpdated -= this.OnDatabaseUpdated;
                    _databaseManager.PropertyChanged -= this.OnDatabasManagerPropertyChanged;
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
