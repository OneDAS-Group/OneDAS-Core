using OneDas.DataManagement.BlazorExplorer.ViewModels;
using OneDas.DataManagement.Database;

namespace OneDas.DataManagement.BlazorExplorer.Shared
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

		private void OnCampaignContainerSelected(CampaignContainer campaignContainer)
		{
			this.AppState.CampaignContainer = campaignContainer;
		}

		#endregion
	}
}
