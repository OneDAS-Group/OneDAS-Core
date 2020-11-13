using Microsoft.AspNetCore.Components;
using OneDas.DataManagement.Explorer.Core;
using OneDas.DataManagement.Explorer.Services;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Explorer.Shared
{
    public partial class JobsSettings
    {
        #region Constructors

		public JobsSettings()
        {
			this.PropertyChanged = (sender, e) =>
			{
                if (e.PropertyName == "Jobs")
                {
                    this.InvokeAsync(() =>
                    {
                        this.StateHasChanged();
                    });
                }
            };
		}

        #endregion

        #region Properties

        [Inject]
        public JobService<ExportJob> ExportJobsService { get; set; }

		[Inject]
		public JobService<AggregationJob> AggregationJobsService { get; set; }

		#endregion

		#region Methods

		protected override Task OnParametersSetAsync()
		{
			this.ExportJobsService.PropertyChanged += this.PropertyChanged;
			this.AggregationJobsService.PropertyChanged += this.PropertyChanged;
			return base.OnParametersSetAsync();
		}

		#endregion

		#region IDisposable Support

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.ExportJobsService.PropertyChanged -= this.PropertyChanged;
				this.AggregationJobsService.PropertyChanged -= this.PropertyChanged;
				base.Dispose(disposing);
			}
		}

		#endregion
	}
}
