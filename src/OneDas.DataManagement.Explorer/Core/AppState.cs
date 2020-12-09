using Microsoft.Extensions.Logging;
using OneDas.DataManagement.Explorer.Core;
using OneDas.DataManagement.Explorer.ViewModels;
using OneDas.DataManagement.Infrastructure;
using OneDas.Extension.Csv;
using OneDas.Infrastructure;
using Prism.Mvvm;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace OneDas.DataManagement.Explorer.Core
{
    public class AppState : BindableBase
    {
        #region Constructors

        public AppState(ILogger<AppState> logger,
                                 OneDasExplorerOptions options)
        {
            this.Logger = logger; 

            this.Version = Assembly.GetEntryAssembly().GetName().Version.ToString();
            this.FileGranularityValues = Utilities.GetEnumValues<FileGranularity>();
            this.FileFormatValues = Utilities.GetEnumValues<FileFormat>();
            this.CodeLanguageValues = Utilities.GetEnumValues<CodeLanguage>();
            this.CsvRowIndexFormatValues = Utilities.GetEnumValues<CsvRowIndexFormat>();
            this.NewsPaper = NewsPaper.Load(Path.Combine(options.DataBaseFolderPath, "news.json"));

            // load filter settings
            var filterSettingsFilePath = Path.Combine(options.DataBaseFolderPath, "filters.json");
            this.FilterSettings = new FilterSettingsViewModel(filterSettingsFilePath);
        }

        #endregion

        #region Properties - General

        public ILogger<AppState> Logger { get; }

        public string Version { get; }

        #endregion

        #region Properties - Settings

        public List<FileGranularity> FileGranularityValues { get; }

        public List<FileFormat> FileFormatValues { get; }

        public List<CsvRowIndexFormat> CsvRowIndexFormatValues { get; }

        #endregion

        #region Properties - Filter

        public List<CodeLanguage> CodeLanguageValues { get; set; }

        public FilterSettingsViewModel FilterSettings { get; }

        #endregion

        #region Properties - News

        public NewsPaper NewsPaper { get; }

        #endregion
    }
}
