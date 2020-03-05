using OneDas.DataManagement.BlazorExplorer.Core;
using OneDas.DataManagement.BlazorExplorer.ViewModels;

namespace OneDas.DataManagement.BlazorExplorer.Shared
{
	public partial class DownloadBox
    {
        #region Constructors

        public DownloadBox()
        {
			this.PropertyChanged = (sender, e) =>
			{
				if (e.PropertyName == nameof(AppStateViewModel.ExportConfiguration))
				{
					this.InvokeAsync(() => { this.StateHasChanged(); });
				}
				else if (e.PropertyName == nameof(AppStateViewModel.DateTimeBegin))
				{
					this.InvokeAsync(() => { this.StateHasChanged(); });
				}
				else if (e.PropertyName == nameof(AppStateViewModel.DateTimeEnd))
				{
					this.InvokeAsync(() => { this.StateHasChanged(); });
				}
				else if (e.PropertyName == nameof(AppStateViewModel.SelectedDatasets))
				{
					this.InvokeAsync(() => { this.StateHasChanged(); });
				}
			};
		}

		#endregion

		#region Methods

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
