using OneDas.DataManagement.Explorer.ViewModels;

namespace OneDas.DataManagement.Explorer.Pages
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
					this.InvokeAsync(this.StateHasChanged);
				}
			};
		}

		#endregion
	}
}
