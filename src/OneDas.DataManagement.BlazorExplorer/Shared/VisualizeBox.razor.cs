using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OneDas.DataManagement.BlazorExplorer.Core;
using OneDas.DataManagement.BlazorExplorer.ViewModels;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDas.DataManagement.BlazorExplorer.Shared
{
	public partial class VisualizeBox
    {
		#region Fields

		private bool _showProgressBar;
		private bool _showWarning;

		#endregion

		#region Constructors

		public VisualizeBox()
		{
			_showProgressBar = true;

			this.PropertyChanged = async (sender, e) =>
			{
				_showWarning = false;

				if (this.AppState.IsSizeLimitExceeded())
				{
					_showWarning = true;
					await this.InvokeAsync(() => { this.StateHasChanged(); });
				}
				else if (!this.AppState.CanVisualize())
				{
					this.AppState.ClientState = ClientState.Normal;
				}
				else
				{
					_showProgressBar = true;

					if (e.PropertyName == nameof(AppStateViewModel.DateTimeBegin))
					{
						await this.InvokeAsync(() => { this.StateHasChanged(); });
						await this.UpdateChartAsync();
					}
					else if (e.PropertyName == nameof(AppStateViewModel.DateTimeEnd))
					{
						await this.InvokeAsync(() => { this.StateHasChanged(); });
						await this.UpdateChartAsync();
					}
					else if (e.PropertyName == nameof(AppStateViewModel.SelectedDatasets))
					{
						await this.InvokeAsync(() => { this.StateHasChanged(); });
						await this.UpdateChartAsync();
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

		private async Task<List<ChartEntry>> BuildChartEntriesAsync()
		{
			var chartEntries = new List<ChartEntry>();

			foreach (var dataset in this.AppState.SelectedDatasets.ToList())
			{
				var name = dataset.Parent.Name;
				var datasetNameParts = dataset.Name.Split('_');

				if (datasetNameParts.Count() == 2)
					name += $" ({datasetNameParts[1]})";

				var data = await this.AppState.LoadDatasetAsync(dataset);

				// remove nans
				for (int i = 0; i < data.Length; i++)
				{
					if (double.IsNaN(data[i]) || double.IsInfinity(data[i]))
						data[i] = 0;
				}

				chartEntries.Add(new ChartEntry(name, dataset.Parent.Unit, data));
			}

			return chartEntries;
		}

		private double[] BuildTimeData()
		{
			var begin = this.AppState.DateTimeBegin.ToUnixTimeStamp();
			var end = this.AppState.DateTimeEnd.ToUnixTimeStamp();
			var sampleRate = (double)new SampleRateContainer(this.AppState.SampleRate).SamplesPerSecond;
			var dt = 1 / sampleRate;
			var count = (int)((end - begin) * sampleRate);

			return Enumerable.Range(0, count).Select(i => begin + i * dt).ToArray();
		}

		private async Task UpdateChartAsync()
		{
			var timeData = this.BuildTimeData();
			var chartEntries = await this.BuildChartEntriesAsync();

			_showProgressBar = false;

			await this.InvokeAsync(() => { this.StateHasChanged(); });
			await JsInterop.UpdateChartAsync(this.JsRuntime, timeData, chartEntries);
		}

		#endregion
	}
}
