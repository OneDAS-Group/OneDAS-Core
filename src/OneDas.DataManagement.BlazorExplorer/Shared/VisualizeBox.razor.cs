using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OneDas.DataManagement.BlazorExplorer.Core;
using OneDas.DataManagement.BlazorExplorer.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace OneDas.DataManagement.BlazorExplorer.Shared
{
	public partial class VisualizeBox
    {
		#region Constructors

		public VisualizeBox()
		{
			this.PropertyChanged = (sender, e) =>
			{
				if (e.PropertyName == nameof(AppStateViewModel.DateTimeBegin))
				{
					this.InvokeAsync(async () =>
					{
						await JsInteropHelper.UpdateChartDataAsync(this.JsRuntime);
					});
				}
				else if (e.PropertyName == nameof(AppStateViewModel.DateTimeEnd))
				{
					this.InvokeAsync(async  () =>
					{
						await JsInteropHelper.UpdateChartDataAsync(this.JsRuntime);
					});
				}
			};
		}

		#endregion

		#region Properties

		[Inject]
		public IJSRuntime JsRuntime { get; set; }

		#endregion

		#region Methods

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if (firstRender)
			{
				var chartEntries = this.AppState.SelectedDatasets.Select(dataset =>
				{
					var name = dataset.Parent.Name;
					var datasetNameParts = dataset.Name.Split('_');

					if (datasetNameParts.Count() == 3)
						name += $" ({datasetNameParts[2]})";

					return new ChartEntry(name, dataset.Parent.Unit);
				}).ToList();

				await JsInteropHelper.DrawChartAsync(this.JsRuntime, chartEntries);
			}

			await base.OnAfterRenderAsync(firstRender);
		}

		#endregion
	}
}
