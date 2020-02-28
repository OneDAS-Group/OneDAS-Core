using Microsoft.AspNetCore.Components;
using OneDas.DataManagement.BlazorExplorer.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace OneDas.DataManagement.BlazorExplorer.Shared
{
    public partial class VariableExpander : IDisposable
    {
        #region Fields

        private List<DatasetInfoViewModel> _filteredDatasets;
        private PropertyChangedEventHandler _propertyChanged;

        #endregion

        #region Properties

        [Inject]
        public AppState AppState { get; set; }

        [Parameter]
        public bool IsExpanded { get; set; }

        [Parameter]
        public VariableInfoViewModel Variable { get; set; }

        #endregion

        #region Methods

        protected override Task OnParametersSetAsync()
        {
            _propertyChanged = (sender, e) =>
            {
                if (e.PropertyName == nameof(AppState.SampleRate))
                {
                    this.InvokeAsync(() =>
                    {
                        this.UpdateFilteredDatasets();
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
                _filteredDatasets = this.Variable.Datasets.Where(dataset => dataset.Name.Contains(this.AppState.SampleRate)).ToList();
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
