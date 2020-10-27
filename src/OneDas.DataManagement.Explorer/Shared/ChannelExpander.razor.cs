using Microsoft.AspNetCore.Components;
using OneDas.DataManagement.Explorer.Core;
using OneDas.DataManagement.Explorer.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Explorer.Shared
{
    public partial class ChannelExpander : IDisposable
    {
        #region Fields

        private bool _showSampleRates;
        private List<DatasetInfoViewModel> _filteredDatasets;
        private PropertyChangedEventHandler _propertyChanged;

        #endregion

        #region Properties

        [Inject]
        public AppStateViewModel AppState { get; set; }

        [Parameter]
        public bool IsExpanded { get; set; }

        [Parameter]
        public ChannelInfoViewModel Channel { get; set; }

        #endregion

        #region Methods

        protected override Task OnParametersSetAsync()
        {
            _propertyChanged = (sender, e) =>
            {
                if (e.PropertyName == nameof(AppStateViewModel.SampleRate))
                {
                    this.InvokeAsync(() =>
                    {
                        this.UpdateFilteredDatasets();
                        this.StateHasChanged();
                    });
                }
                else if (e.PropertyName == nameof(AppStateViewModel.IsEditEnabled))
                {
                    this.InvokeAsync(() =>
                    {
                        this.StateHasChanged();
                    });
                }
            };

            this.UpdateFilteredDatasets();
            this.AppState.PropertyChanged += _propertyChanged;

            return base.OnParametersSetAsync();
        }

        private void OnClick()
        {
            this.IsExpanded = !this.IsExpanded;
        }

        private void UpdateFilteredDatasets()
        {
            if (string.IsNullOrWhiteSpace(this.AppState.SampleRate))
                _filteredDatasets = new List<DatasetInfoViewModel>();
            else
                _filteredDatasets = this.Channel.Datasets.Where(dataset => dataset.Name.Contains(this.AppState.SampleRate)).ToList();
        }

        private List<string> GetSampleRates()
        {
            return this.Channel.Datasets
                .Select(dataset => dataset.Name.Split('_')[0])
                .Distinct()
                .Where(sampleRate => sampleRate != this.AppState.SampleRate)
                .OrderBy(x => x, new SampleRateStringComparer()).ToList();
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
