using MatBlazor;
using OneDas.DataManagement.Explorer.ViewModels;
using System.Collections.Generic;

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
				if (e.PropertyName == nameof(AppStateViewModel.SearchString))
				{
					this.InvokeAsync(() =>
					{
						_searchIcon = string.IsNullOrWhiteSpace(this.AppState.SearchString) ? MatIconNames.Search : MatIconNames.Close;
						this.StateHasChanged();
					});
				}
				else if (e.PropertyName == nameof(AppStateViewModel.CampaignContainer))
				{
					this.InvokeAsync(() =>
					{
						this.GroupPage = 0;
						this.StateHasChanged();
					});
				}
				else if (e.PropertyName == nameof(AppStateViewModel.VariableGroup))
				{
					this.VariablePage = 0;
				}
			};
		}

		#endregion

		#region Properties

		public int GroupPageSize { get; set; } = 15;

		public int GroupPage { get; set; } = 0;

		public int VariablePageSize { get; set; } = 9;

		public int VariablePage { get; set; } = 0;

		#endregion

		#region Methods

		public void OnVariableGroupSelected(List<VariableInfoViewModel> variableGroup)
		{
			this.AppState.VariableGroup = variableGroup;
		}

		#endregion
    }
}
