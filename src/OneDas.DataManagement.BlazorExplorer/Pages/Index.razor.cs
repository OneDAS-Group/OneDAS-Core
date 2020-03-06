using OneDas.DataManagement.BlazorExplorer.ViewModels;

namespace OneDas.DataManagement.BlazorExplorer.Pages
{
    public partial class Index
	{
		#region Constructors

		public Index()
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
	}
}
