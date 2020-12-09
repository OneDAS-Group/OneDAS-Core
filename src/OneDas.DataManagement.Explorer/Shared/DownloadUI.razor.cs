using OneDas.DataManagement.Explorer.ViewModels;

namespace OneDas.DataManagement.Explorer.Shared
{
    public partial class DownloadUI
    {
		#region Constructors

		public DownloadUI()
		{
			this.PropertyChanged = (sender, e) =>
			{
				if (e.PropertyName == nameof(UserState.DownloadMessage))
				{
					this.InvokeAsync(this.StateHasChanged);
				}
				else if (e.PropertyName == nameof(UserState.DownloadProgress))
				{
					this.InvokeAsync(this.StateHasChanged);
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
			this.UserState.CancelDownload();
		}
		
		#endregion
	}
}
