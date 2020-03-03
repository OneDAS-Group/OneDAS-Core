using OneDas.DataManagement.BlazorExplorer.ViewModels;

namespace OneDas.DataManagement.BlazorExplorer.Shared
{
    public partial class DownloadUI
    {
		#region Constructors

		public DownloadUI()
		{
			this.PropertyChanged = (sender, e) =>
			{
				if (e.PropertyName == nameof(AppStateViewModel.DownloadMessage))
				{
					this.InvokeAsync(() =>
					{
						this.StateHasChanged();
					});
				}
				else if (e.PropertyName == nameof(AppStateViewModel.DownloadProgress))
				{
					this.InvokeAsync(() =>
					{
						this.StateHasChanged();
					});
				}
			};
		}

		#endregion

		#region Properties

		public bool IsCancelling { get; private set; }

		#endregion

		#region Methods

		public void CancelDownload()
		{
			this.IsCancelling = true;
			this.AppState.CancelDownload();
		}
		
		#endregion
	}
}
