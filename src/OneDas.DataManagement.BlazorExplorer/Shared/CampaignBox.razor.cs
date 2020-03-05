using OneDas.DataManagement.BlazorExplorer.ViewModels;
using OneDas.DataManagement.Database;

namespace OneDas.DataManagement.BlazorExplorer.Shared
{
	public partial class CampaignBox
    {
		#region Properties

		public bool AttachmentDialogIsOpen { get; set; }

		#endregion

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

		#region Methods

		public void OpenAttachmentDialog()
		{
			this.AttachmentDialogIsOpen = true;
		}

		public void OnCampaignContainerSelected(CampaignContainer campaignContainer)
		{
			this.AppState.CampaignContainer = campaignContainer;
		}

		#endregion
	}
}
