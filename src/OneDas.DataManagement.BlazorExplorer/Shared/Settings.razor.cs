using BlazorInputFile;
using Microsoft.AspNetCore.Components;
using OneDas.DataManagement.BlazorExplorer.Core;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace OneDas.DataManagement.BlazorExplorer.Shared
{
    public partial class Settings : IDisposable
    {
        #region Fields

        private PropertyChangedEventHandler _propertyChanged;

        #endregion

        #region Properties

        [Inject]
        public AppStateViewModel AppState { get; set; }

		#endregion

		#region Methods

		protected override Task OnParametersSetAsync()
		{
			_propertyChanged = (sender, e) =>
			{
				if (e.PropertyName == nameof(AppStateViewModel))
				{
					this.InvokeAsync(() =>
					{
						this.StateHasChanged();
					});
				}
			};

			this.AppState.PropertyChanged += _propertyChanged;

			return base.OnParametersSetAsync();
		}

		private void OnSaveExportSettings()
        {
            var jsonString = JsonSerializer.Serialize(this.AppState._model, new JsonSerializerOptions() { WriteIndented = true });
            File.WriteAllText("settings.json", jsonString);
        }

        private async Task OnLoadExportSettings(IFileListEntry[] files)
        {
            var file = files.FirstOrDefault();

            if (file != null)
            {
                var exportConfiguration = await JsonSerializer.DeserializeAsync<ExportConfiguration>(file.Data);
                this.AppState.SetExportConfiguration(exportConfiguration);
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
					this.AppState.PropertyChanged -= _propertyChanged;
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
