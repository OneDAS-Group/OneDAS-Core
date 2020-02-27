using Microsoft.AspNetCore.Components;
using OneDas.DataManagement.BlazorExplorer.Core;
using OneDas.DataManagement.Database;
using System.Collections.Generic;

namespace OneDas.DataManagement.BlazorExplorer.Pages
{
	public partial class Index
	{
		#region Properties

		[Inject]
		public AppState AppState { get; set; }

		public bool attachmentDialogIsOpen { get; set; }

		#endregion

		#region Methods

		public void OpenattachmentDialog()
		{
			this.attachmentDialogIsOpen = true;
		}

		public void OnCampaignContainerSelected(CampaignContainer campaignContainer)
		{
			this.AppState.CampaignContainer = campaignContainer;
		}

		public void OnVariableGroupSelected(List<VariableInfoViewModel> variableGroup)
		{
			this.AppState.VariableGroup = variableGroup;
		}

		#endregion
	}
}
