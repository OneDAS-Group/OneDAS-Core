namespace OneDas.DataManagement.Explorer.Shared
{
    public partial class ProjectBox
    {
		#region Constructors

		public ProjectBox()
		{
			this.PropertyChanged = (sender, e) =>
			{
				if (e.PropertyName == nameof(UserState.ClientState))
				{
					this.InvokeAsync(this.StateHasChanged);
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

		public bool LogBookDialogIsOpen { get; set; }

		public bool AttachmentsDialogIsOpen { get; set; }

		#endregion

		#region Methods

		private void OpenLogBookDialog()
		{
			this.LogBookDialogIsOpen = true;
		}

		private void OpenAttachmentsDialog()
		{
			this.AttachmentsDialogIsOpen = true;
		}

		#endregion
	}
}
