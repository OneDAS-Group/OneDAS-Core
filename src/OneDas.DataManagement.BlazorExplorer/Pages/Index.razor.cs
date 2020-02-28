using MatBlazor;
using Microsoft.AspNetCore.Components;
using OneDas.DataManagement.BlazorExplorer.Core;
using OneDas.DataManagement.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace OneDas.DataManagement.BlazorExplorer.Pages
{
	public partial class Index : IDisposable
	{
		#region Fields

		private string _searchIcon = MatIconNames.Search;
		private PropertyChangedEventHandler _propertyChanged;

		#endregion

		#region Properties

		[Inject]
		public AppState AppState { get; set; }

		public bool AttachmentDialogIsOpen { get; set; }

		public int GroupPageSize { get; set; } = 15;

		public int GroupPage { get; set; } = 0;

		public int VariablePageSize { get; set; } = 10;

		public int VariablePage { get; set; } = 0;

		#endregion

		#region Methods

		protected override Task OnParametersSetAsync()
		{
			_propertyChanged = (sender, e) =>
			{
				if (e.PropertyName == nameof(AppState.SearchString))
				{
					this.InvokeAsync(() =>
					{
						_searchIcon = string.IsNullOrWhiteSpace(this.AppState.SearchString) ? MatIconNames.Search : MatIconNames.Close;
						this.StateHasChanged();
					});
				}
				else if (e.PropertyName == nameof(AppState.CampaignContainer))
				{
					this.GroupPage = 0;
				}
				else if (e.PropertyName == nameof(AppState.VariableGroup))
				{
					this.VariablePage = 0;
				}
				else if (e.PropertyName == nameof(AppState.SelectedDatasets))
				{
					this.InvokeAsync(() =>
					{
						this.StateHasChanged();
					});
				}
			};

			this.AppState.PropertyChanged += _propertyChanged;

			return base.OnParametersSetAsync();
		}

		public void OpenAttachmentDialog()
		{
			this.AttachmentDialogIsOpen = true;
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

		#region IDisposable Support

		private bool disposedValue = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					this.AppState.PropertyChanged -= _propertyChanged;
				}

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}

		#endregion
	}
}
