using Microsoft.AspNetCore.Components;
using OneDas.DataManagement.BlazorExplorer.Core;
using OneDas.DataManagement.BlazorExplorer.ViewModels;
using System;
using System.IO;
using System.Text.Json;

namespace OneDas.DataManagement.BlazorExplorer.Shared
{
    public partial class Presets
    {
        #region Properties

        [Inject]
        public AppStateViewModel AppState { get; set; }

        [Parameter]
        public bool IsOpen { get; set; }

        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

        #endregion

        #region Methods

        private string GetFileName(string filePath)
        {
            return Path.GetFileName(filePath);
        }

        private void LoadPreset(string filePath)
        {
            var jsonString = File.ReadAllText(filePath);
            var exportConfiguration = JsonSerializer.Deserialize<ExportConfiguration>(jsonString);

            var timeSpan = exportConfiguration.DateTimeEnd - exportConfiguration.DateTimeBegin;
            var dt = exportConfiguration.DateTimeEnd;
            var now = DateTime.UtcNow.AddDays(-1);

            exportConfiguration.DateTimeEnd = new DateTime(now.Year, now.Month, now.Day, dt.Hour, dt.Minute, dt.Second); ;
            exportConfiguration.DateTimeBegin = exportConfiguration.DateTimeEnd - timeSpan;

            this.AppState.SetExportConfiguration(exportConfiguration);
        }

        private void OnIsOpenChanged(bool value)
        {
            this.IsOpen = value;
            this.IsOpenChanged.InvokeAsync(value);
        }

        #endregion
    }
}
