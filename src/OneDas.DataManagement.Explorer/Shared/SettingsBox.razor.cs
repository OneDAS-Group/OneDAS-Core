using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using OneDas.DataManagement.Explorer.Core;
using OneDas.DataManagement.Explorer.ViewModels;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Explorer.Shared
{
    public partial class SettingsBox
    {
        #region Constructors

        public SettingsBox()
        {
            this.PropertyChanged = (sender, e) =>
            {
                if (e.PropertyName == nameof(AppStateViewModel.ExportConfiguration))
                {
                    this.InvokeAsync(this.StateHasChanged);
                }
                else if (e.PropertyName == nameof(AppStateViewModel.SelectedDatasets))
                {
                    this.InvokeAsync(this.StateHasChanged);
                }
            };
        }

        #endregion

        #region Properties

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        public bool PresetsDialogIsOpen { get; set; }

        #endregion

        #region Methods

        private void OpenPresetsDialog()
        {
            this.PresetsDialogIsOpen = true;
        }

        private async Task OnSaveExportSettingsAsync()
        {
			var configuration = this.AppState.ExportConfiguration;
			var jsonString = JsonSerializer.Serialize(configuration, new JsonSerializerOptions() { WriteIndented = true });
			await JsInterop.BlobSaveAs(this.JsRuntime, "export.json", Encoding.UTF8.GetBytes(jsonString));
		}

        private async Task OnLoadExportSettingsAsync(InputFileChangeEventArgs e)
        {
            var file = e.File;

            if (file != null)
            {
                using var utf8json = file.OpenReadStream();
                var exportConfiguration = await JsonSerializer.DeserializeAsync<ExportConfiguration>(utf8json);
                exportConfiguration = ExportConfiguration.UpdateVersion(exportConfiguration);
                this.AppState.SetExportConfiguration(exportConfiguration);
            }
        }

		#endregion
	}
}
