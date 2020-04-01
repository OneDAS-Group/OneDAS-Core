using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OneDas.DataManagement.Explorer.Core;
using OneDas.DataManagement.Explorer.ViewModels;
using OneDas.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Explorer.Shared
{
    public partial class VisualizeBox
    {
		#region Fields

		private bool _showWarning;

		#endregion

		#region Constructors

		public VisualizeBox()
		{
			this.PropertyChanged = async (sender, e) =>
			{
				_showWarning = false;

				if (this.AppState.IsSizeLimitExceeded())
				{
					_showWarning = true;
					await this.InvokeAsync(this.StateHasChanged);
				}
				else if (!this.AppState.CanVisualize())
				{
					this.AppState.ClientState = ClientState.Normal;
				}
				else
				{
					if (e.PropertyName == nameof(AppStateViewModel.ExportConfiguration))
					{
						await this.UpdateChartAsync();
					}
					else if (e.PropertyName == nameof(AppStateViewModel.DateTimeBegin))
					{
						await this.UpdateChartAsync();
					}
					else if (e.PropertyName == nameof(AppStateViewModel.DateTimeEnd))
					{
						await this.UpdateChartAsync();
					}
					else if (e.PropertyName == nameof(AppStateViewModel.SelectedDatasets))
					{
						await this.UpdateChartAsync();
					}
					else if (e.PropertyName == nameof(AppStateViewModel.VisualizeBeginAtZero))
					{
						await this.UpdateChartAsync();
					}
					else if (e.PropertyName == nameof(AppStateViewModel.VisualizeProgress))
					{
						await this.InvokeAsync(this.StateHasChanged);
					}
				}
			};
		}

		#endregion

		#region Properties

		[Inject]
		public IJSRuntime JsRuntime { get; set; }

		#endregion

		#region Methods

		protected override void OnInitialized()
		{
			if (this.AppState.IsSizeLimitExceeded())
				_showWarning = true;

			base.OnInitialized();
		}

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if (firstRender)
				await this.UpdateChartAsync();

			await base.OnAfterRenderAsync(firstRender);
		}

		private List<ChartEntry> BuildChartEntriesAsync()
		{
			var chartEntries = new List<ChartEntry>();

			foreach (var dataset in this.AppState.SelectedDatasets.ToList())
			{
				var name = dataset.Parent.Name;
				var datasetNameParts = dataset.Name.Split('_');

				if (datasetNameParts.Count() == 2)
					name += $" ({datasetNameParts[1]})";

				var path = dataset.Model.GetPath();

				chartEntries.Add(new ChartEntry(name, path, dataset.Parent.Unit));
			}

			return chartEntries;
		}

		private async Task UpdateChartAsync()
		{
			var chartEntries = this.BuildChartEntriesAsync();
			var begin = this.AppState.DateTimeBegin;
			var end = this.AppState.DateTimeEnd;
			var sampleRate = (double)new SampleRateContainer(this.AppState.SampleRate).SamplesPerSecond;
			var dt = 1 / sampleRate;

			var count = (int)((end - begin).TotalSeconds * sampleRate);

			await this.InvokeAsync(this.StateHasChanged);
			await JsInterop.UpdateChartAsync(this.JsRuntime, this.AppState, chartEntries, begin, end, count, dt, this.AppState.VisualizeBeginAtZero);
		}

		#endregion
	}
}
