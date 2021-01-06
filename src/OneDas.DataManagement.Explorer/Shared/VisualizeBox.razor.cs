using MatBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using OneDas.DataManagement.Explorer.Core;
using OneDas.DataManagement.Explorer.Services;
using OneDas.Infrastructure;
using OneDas.Types;
using System;
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

				if (this.UserState.IsSizeLimitExceeded())
				{
					_showWarning = true;
					await this.InvokeAsync(this.StateHasChanged);
				}
				else if (!this.UserState.CanVisualize())
				{
					this.UserState.ClientState = ClientState.Normal;
				}
				else
				{
					if (e.PropertyName == nameof(UserState.ExportParameters))
					{
						await this.UpdateChartAsync();
					}
					else if (e.PropertyName == nameof(UserState.DateTimeBegin))
					{
						await this.UpdateChartAsync();
					}
					else if (e.PropertyName == nameof(UserState.DateTimeEnd))
					{
						await this.UpdateChartAsync();
					}
					else if (e.PropertyName == nameof(UserState.SelectedDatasets))
					{
						await this.UpdateChartAsync();
					}
					else if (e.PropertyName == nameof(UserState.VisualizeBeginAtZero))
					{
						await this.UpdateChartAsync();
					}
					else if (e.PropertyName == nameof(UserState.VisualizeProgress))
					{
						await this.InvokeAsync(this.StateHasChanged);
					}
				}
			};
		}

		#endregion

		#region Properties

		[Inject]
		public ToasterService ToasterService { get; set; }

		[Inject]
		public IJSRuntime JsRuntime { get; set; }

		#endregion

		#region Methods

		protected override void OnInitialized()
		{
			if (this.UserState.IsSizeLimitExceeded())
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

			foreach (var dataset in this.UserState.SelectedDatasets.ToList())
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
            try
            {
				var chartEntries = this.BuildChartEntriesAsync();
				var begin = this.UserState.DateTimeBegin;
				var end = this.UserState.DateTimeEnd;
				var sampleRate = (double)new SampleRateContainer(this.UserState.SampleRate).SamplesPerSecond;
				var dt = 1 / sampleRate;

				var count = (int)((end - begin).TotalSeconds * sampleRate);

				await this.InvokeAsync(this.StateHasChanged);
				await this.JsRuntime.UpdateChartAsync(this.UserState, chartEntries, begin, end, count, dt, this.UserState.VisualizeBeginAtZero);
			}
            catch (TaskCanceledException)
            {
				// prevent that the whole app crashes in the followig case:
				// - OneDAS calculates aggregations and locks current file
				// GUI wants to load data from that locked file and times out
				// TaskCanceledException is thrown: app crashes.
				this.UserState.ClientState = ClientState.Normal;
			}
			catch (UnauthorizedAccessException ex)
            {
				this.UserState.Logger.LogError(ex.GetFullMessage());
				this.ToasterService.ShowError(message: "Unauthorized.", icon: MatIconNames.Lock);
				this.UserState.ClientState = ClientState.Normal;
			}
			catch (Exception ex)
			{
				this.UserState.Logger.LogError(ex.GetFullMessage());
				this.ToasterService.ShowError(message: "Unable to stream data.", icon: MatIconNames.Error_outline);
				this.UserState.ClientState = ClientState.Normal;
			}
		}

		#endregion
	}
}
