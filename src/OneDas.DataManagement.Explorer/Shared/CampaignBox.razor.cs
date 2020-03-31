using OneDas.DataManagement.Explorer.ViewModels;

namespace OneDas.DataManagement.Explorer.Shared
{
    public partial class CampaignBox
    {
		#region Constructors

		public CampaignBox()
		{
			this.PropertyChanged = (sender, e) =>
			{
				if (e.PropertyName == nameof(AppStateViewModel.ClientState))
				{
					this.InvokeAsync(() => { this.StateHasChanged(); });
				}
			};
		}

		#endregion

		#region Properties

		public bool AttachmentsDialogIsOpen { get; set; }

		#endregion

		#region Methods

		private void OpenAttachmentsDialog()
		{
			this.AttachmentsDialogIsOpen = true;
		}

		#endregion
	}
}
