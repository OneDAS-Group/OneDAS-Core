using MatBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using OneDas.DataManagement.Explorer.Core;
using OneDas.DataManagement.Explorer.Services;
using OneDas.DataManagement.Explorer.ViewModels;
using OneDas.Types;
using System;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Explorer.Shared
{
    public partial class DownloadBox
    {
        #region Constructors

        public DownloadBox()
        {
			this.PropertyChanged = (sender, e) =>
			{
				if (e.PropertyName == nameof(AppStateViewModel.ExportParameters))
				{
					this.InvokeAsync(this.StateHasChanged);
				}
				else if (e.PropertyName == nameof(AppStateViewModel.DateTimeBegin))
				{
					this.InvokeAsync(this.StateHasChanged);
				}
				else if (e.PropertyName == nameof(AppStateViewModel.DateTimeEnd))
				{
					this.InvokeAsync(this.StateHasChanged);
				}
				else if (e.PropertyName == nameof(AppStateViewModel.FileGranularity))
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

		#region Properties - Injected

		[Inject]
		public ToasterService ToasterService { get; set; }

		#endregion

		#region Methods

		private async Task DownloadAsync()
        {
            try
            {
				await this.AppState.DownloadAsync();
			}
            catch (Exception ex)
            {
				this.AppState.Logger.LogError(ex.GetFullMessage());
				this.ToasterService.ShowError(message: "Unable to download data.", icon: MatIconNames.Error_outline);
			}
        }

		private string GetDownloadLabel()
		{
			var byteCount = this.AppState.GetByteCount();

			if (byteCount > 0)
				return $"Download ({Utilities.FormatByteCount(byteCount)})";
			else
				return $"Download";
		}

		#endregion
	}
}
