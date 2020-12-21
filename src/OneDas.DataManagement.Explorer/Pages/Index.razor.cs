namespace OneDas.DataManagement.Explorer.Pages
{
    public partial class Index
	{
		#region Constructors

		public Index()
		{
			this.PropertyChanged = (sender, e) =>
			{
				if (e.PropertyName == nameof(UserState.ClientState))
				{
					this.InvokeAsync(this.StateHasChanged);
				}
			};
		}

        #endregion
    }
}
