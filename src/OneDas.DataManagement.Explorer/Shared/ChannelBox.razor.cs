using MatBlazor;

namespace OneDas.DataManagement.Explorer.Shared
{
    public partial class ChannelBox
    {
		#region Fields

		private string _searchIcon = MatIconNames.Search;

		#endregion

		#region Constructors

		public ChannelBox()
		{
			this.PropertyChanged = (sender, e) =>
			{
				if (e.PropertyName == nameof(UserState.SearchString))
				{
					this.InvokeAsync(() =>
					{
                        _searchIcon = string.IsNullOrWhiteSpace(this.UserState.SearchString) ? MatIconNames.Search : MatIconNames.Close;
						this.StateHasChanged();
					});
				}
				else if (e.PropertyName == nameof(UserState.ProjectContainer))
				{
					this.InvokeAsync(() =>
					{
						this.GroupPage = 0;
						this.StateHasChanged();
					});
				}
				else if (e.PropertyName == nameof(UserState.GroupedChannelsEntry))
				{
					this.ChannelPage = 0;
				}
				else if (e.PropertyName == nameof(AppState.IsDatabaseInitialized) ||
						(e.PropertyName == nameof(AppState.IsDatabaseUpdating) && !this.AppState.IsDatabaseUpdating))
				{
					this.InvokeAsync(this.StateHasChanged);
				}
			};
		}

		#endregion

		#region Properties

		public bool LicenseDialogIsOpen { get; set; }

		public int GroupPageSize { get; set; } = 15;

		public int GroupPage { get; set; } = 0;

		public int ChannelPageSize { get; set; } = 9;

		public int ChannelPage { get; set; } = 0;

		#endregion

		#region Methods

		private void OpenLicenseDialog()
		{
			this.LicenseDialogIsOpen = true;
		}

		#endregion
	}
}
