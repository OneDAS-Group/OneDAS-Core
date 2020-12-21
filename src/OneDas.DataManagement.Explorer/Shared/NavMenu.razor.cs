namespace OneDas.DataManagement.Explorer.Shared
{
    public partial class NavMenu
    {
        public NavMenu()
        {
			this.PropertyChanged = (sender, e) =>
			{
				if (e.PropertyName == nameof(AppState.IsDatabaseUpdating))
				{
					this.InvokeAsync(this.StateHasChanged);
				}
			};
		}
    }
}
