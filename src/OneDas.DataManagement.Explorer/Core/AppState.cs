using Microsoft.Extensions.Logging;
using OneDas.DataManagement.Database;
using OneDas.DataManagement.Explorer.Services;
using OneDas.DataManagement.Explorer.ViewModels;
using OneDas.DataManagement.Infrastructure;
using OneDas.Extension.Csv;
using OneDas.Infrastructure;
using OneDas.Types;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Explorer.Core
{
    public class AppState : BindableBase
    {
        #region Fields

        private UserManager _userManager;
        private OneDasExplorerOptions _options;
        private bool _isAppInitialized;
        private bool _isDatabaseInitialized;
        private bool _isDatabaseUpdating;
        private SemaphoreSlim _updateDatabaseSemaphore;
        private CancellationTokenSource _updateDatabaseCancellationTokenSource;
        private DatabaseManager _databaseManager;
        private Dictionary<ProjectContainer, List<ChannelInfoViewModel>> _channelCache;

        #endregion

        #region Constructors

        public AppState(ILogger<AppState> logger,
                        DatabaseManager databaseManager,
                        UserManager userManager,
                        OneDasExplorerOptions options)
        {
            this.Logger = logger;
            _databaseManager = databaseManager;
            _userManager = userManager;
            _options = options;

            this.CsvRowIndexFormatValues = Utilities.GetEnumValues<CsvRowIndexFormat>();
            this.CodeLanguageValues = Utilities.GetEnumValues<CodeLanguage>();
            this.CodeTypeValues = Utilities.GetEnumValues<CodeType>();
            this.FileFormatValues = Utilities.GetEnumValues<FileFormat>();
            this.FileGranularityValues = Utilities.GetEnumValues<FileGranularity>();
            this.Version = Assembly.GetEntryAssembly().GetName().Version.ToString();

            if (!string.IsNullOrWhiteSpace(options.DataBaseFolderPath))
            {
                if (!this.TryInitializeApp(out var ex))
                    throw ex;
            }           
        }

        #endregion

        #region Properties - General

        public bool IsAppInitialized
        {
            get { return _isAppInitialized; }
            set { this.SetProperty(ref _isAppInitialized, value); }
        }

        public bool IsDatabaseInitialized
        {
            get { return _isDatabaseInitialized; }
            set { this.SetProperty(ref _isDatabaseInitialized, value); }
        }

        public bool IsDatabaseUpdating
        {
            get { return _isDatabaseUpdating; }
            set { this.SetProperty(ref _isDatabaseUpdating, value); }
        }

        public ILogger<AppState> Logger { get; }

        public string Version { get; }

        #endregion

        #region Properties - Settings

        public List<FileGranularity> FileGranularityValues { get; }

        public List<FileFormat> FileFormatValues { get; }

        public List<CsvRowIndexFormat> CsvRowIndexFormatValues { get; }

        #endregion

        #region Properties - Filter

        public List<CodeType> CodeTypeValues { get; }

        public List<CodeLanguage> CodeLanguageValues { get; }

        public FilterSettingsViewModel FilterSettings { get; private set; }

        #endregion

        #region Properties - News

        public NewsPaper NewsPaper { get; private set; }

        #endregion

        #region Methods

        public bool TryInitializeApp(out Exception exception)
        {
            exception = null;

            try
            {
                Directory.CreateDirectory(_options.DataBaseFolderPath);

                this.NewsPaper = NewsPaper.Load(Path.Combine(_options.DataBaseFolderPath, "news.json"));

                var filterSettingsFilePath = Path.Combine(_options.DataBaseFolderPath, "filters.json");
                this.FilterSettings = new FilterSettingsViewModel(filterSettingsFilePath);
                this.InitializeFilterSettings(this.FilterSettings.Model, filterSettingsFilePath);

                _channelCache = new Dictionary<ProjectContainer, List<ChannelInfoViewModel>>();
                _updateDatabaseSemaphore = new SemaphoreSlim(initialCount: 1, maxCount: 1);

                _userManager.Initialize();
                _options.Save(Program.OptionsFilePath);

                _ = this.UpdateDatabaseAsync();
            }
            catch (Exception ex)
            {
                exception = ex;
                this.Logger.LogError(ex.GetFullMessage());
                return false;
            }

            this.IsAppInitialized = true;
            return true;
        }

        public async Task UpdateDatabaseAsync()
        {
            _updateDatabaseCancellationTokenSource?.Cancel();
            _updateDatabaseCancellationTokenSource = new CancellationTokenSource();

            await _updateDatabaseSemaphore.WaitAsync();

            try
            { 
                this.IsDatabaseUpdating = true;
                await _databaseManager.UpdateAsync(_updateDatabaseCancellationTokenSource.Token);
                _channelCache.Clear();
                this.IsDatabaseInitialized = true;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex.GetFullMessage());
                throw;
            }
            finally
            {
                this.IsDatabaseUpdating = false;
                _updateDatabaseSemaphore.Release();
            }
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

        private void InitializeFilterSettings(FilterSettings filterSettings, string filePath)
        {
            // ensure that code samples of test user are present
            var testCodes = filterSettings.CodeDefinitions.Where(code => code.Owner == "test@onedas.org");

            if (!testCodes.Any(testCode => testCode.Name == "Simple filter (C#)"))
            {
                using var streamReader1 = new StreamReader(ResourceLoader.GetResourceStream("OneDas.DataManagement.Explorer.Resources.TestUserFilterCodeTemplateSimple.cs"));

                filterSettings.CodeDefinitions.Add(new CodeDefinition()
                {
                    Code = streamReader1.ReadToEnd(),
                    CodeLanguage = CodeLanguage.CSharp,
                    CodeType = CodeType.Filter,
                    CreationDate = DateTime.UtcNow,
                    IsPublic = true,
                    Name = "Simple filter (C#)",
                    Owner = "test@onedas.org",
                    RequestedProjectIds = new List<string>() { "/IN_MEMORY/TEST/ACCESSIBLE" },
                    SampleRate = "1 s"
                });

                filterSettings.Save(filePath);
            }

            if (!testCodes.Any(testCode => testCode.Name == "Simple shared (C#)"))
            {
                using var streamReader2 = new StreamReader(ResourceLoader.GetResourceStream("OneDas.DataManagement.Explorer.Resources.TestUserSharedCodeTemplateSimple.cs"));

                filterSettings.CodeDefinitions.Add(new CodeDefinition()
                {
                    Code = streamReader2.ReadToEnd(),
                    CodeLanguage = CodeLanguage.CSharp,
                    CodeType = CodeType.Shared,
                    CreationDate = DateTime.UtcNow,
                    IsPublic = true,
                    Name = "Simple shared (C#)",
                    Owner = "test@onedas.org",
                    RequestedProjectIds = new List<string>(),
                    SampleRate = string.Empty
                });

                filterSettings.Save(filePath);
            }
        }

        #endregion
    }
}
