using Microsoft.AspNetCore.Components;
using OneDas.DataManagement.BlazorExplorer.ViewModels;
using OneDas.DataManagement.Database;

namespace OneDas.DataManagement.BlazorExplorer.Shared
{
	public partial class CampaignBox
    {
		#region Properties

		[Inject]
		public AppStateViewModel AppState { get; set; }

		public bool AttachmentDialogIsOpen { get; set; }

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
