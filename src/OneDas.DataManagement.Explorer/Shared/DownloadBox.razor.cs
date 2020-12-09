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
				if (e.PropertyName == nameof(UserState.ExportParameters))
				{
					this.InvokeAsync(this.StateHasChanged);
				}
				else if (e.PropertyName == nameof(UserState.DateTimeBegin))
				{
					this.InvokeAsync(this.StateHasChanged);
				}
				else if (e.PropertyName == nameof(UserState.DateTimeEnd))
				{
					this.InvokeAsync(this.StateHasChanged);
				}
				else if (e.PropertyName == nameof(UserState.FileGranularity))
				{
					this.InvokeAsync(this.StateHasChanged);
				}
				else if (e.PropertyName == nameof(UserState.SelectedDatasets))
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
				await this.UserState.DownloadAsync();
			}
            catch (Exception ex)
            {
				this.UserState.Logger.LogError(ex.GetFullMessage());
				this.ToasterService.ShowError(message: "Unable to download data.", icon: MatIconNames.Error_outline);
			}
        }

		private string GetDownloadLabel()
		{
			var byteCount = this.UserState.GetByteCount();

			if (byteCount > 0)
				return $"Download ({Utilities.FormatByteCount(byteCount)})";
			else
				return $"Download";
		}

		#endregion
	}
}
