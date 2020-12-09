using MatBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OneDas.DataManagement.Explorer.Core;
using OneDas.DataManagement.Explorer.Services;
using OneDas.DataManagement.Explorer.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using static OneDas.DataManagement.Explorer.Services.MonacoService;

namespace OneDas.DataManagement.Explorer.Pages
{
    public partial class FilterEditor : IDisposable
    {
        #region Fields

        private string _editorId;
        private FilterDescriptionViewModel _filterDescription;
		private DotNetObjectReference<MonacoService> _objRef;

        #endregion

        #region Properties

        public List<Diagnostic> Diagnostics { get; set; }

        [Inject]
        private ToasterService ToasterService { get; set; }

        [Inject]
        private IJSRuntime JS { get; set; }

        [Inject]
        private MonacoService MonacoService { get; set; }

        [Inject]
        private UserIdService UserIdService { get; set; }

        private FilterDescriptionViewModel FilterDescription
        {
            get
            {
                return _filterDescription;
            }
            set
            {
                if (_filterDescription is not null)
                    _filterDescription.PropertyChanged -= this.OnFilterDescriptionPropertyChanged;

                _filterDescription = value;
                _filterDescription.PropertyChanged += this.OnFilterDescriptionPropertyChanged;

                this.MonacoService.SetValues(_filterDescription.Code, _filterDescription.SampleRate);
                _ = this.JS.SetMonacoValueAsync(_editorId, this.FilterDescription.Code);
            }
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            _objRef?.Dispose();
            this.AppState.FilterSettings.PropertyChanged -= this.OnFilterSettingsPropertyChanged;
            this.MonacoService.DiagnosticsUpdated -= this.OnDiagnosticsUpdated;
        }

        protected override void OnParametersSet()
        {
            this.FilterDescription = new FilterDescriptionViewModel(new FilterDescription()
            {
                Name = "NewFilter",
                Group = this.UserIdService.User.Identity.Name,
                Unit = "-",
                SampleRate = this.UserState.SampleRateValues.FirstOrDefault(),
                Code = this.MonacoService.DefaultChannelCode
            });

            base.OnParametersSet();

            this.AppState.FilterSettings.PropertyChanged += this.OnFilterSettingsPropertyChanged;
            this.MonacoService.DiagnosticsUpdated += this.OnDiagnosticsUpdated;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _objRef = DotNetObjectReference.Create(this.MonacoService);
                _editorId = "1";

                var options = new Dictionary<string, object>
                {
                    ["automaticLayout"] = true,
                    ["language"] = "csharp",
                    ["scrollBeyondLastLine"] = false,
                    ["value"] = this.FilterDescription.Code,
                    ["theme"] = "vs-dark"
                };

                await this.JS.CreateMonacoEditorAsync(_editorId, options);
                await this.JS.RegisterMonacoProvidersAsync(_editorId, _objRef);
            }
        }

        #endregion

        #region EventHandlers

        private void OnFilterDescriptionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FilterDescriptionViewModel.CodeType))
            {
                this.FilterDescription.Code = this.FilterDescription.CodeType switch
                {
                    CodeType.Channel    => this.MonacoService.DefaultChannelCode,
                    CodeType.Project    => this.MonacoService.DefaultProjectCode,
                    CodeType.Shared     => this.MonacoService.DefaultSharedCode,
                    _                   => throw new Exception($"The code type '{this.FilterDescription.CodeType}' is not supported.")
                };

                this.MonacoService.SetValues(this.FilterDescription.Code, this.FilterDescription.SampleRate);
                _ = this.JS.SetMonacoValueAsync(_editorId, this.FilterDescription.Code);
            }

            else if (e.PropertyName == nameof(FilterDescriptionViewModel.SampleRate))
            {
                this.MonacoService.SetValues(this.FilterDescription.Code, this.FilterDescription.SampleRate);
            }
        }

        private void OnFilterSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FilterSettingsViewModel.FilterDescriptions))
                this.InvokeAsync(() => { this.StateHasChanged(); });
        }

        private void OnDiagnosticsUpdated(object sender, List<Diagnostic> diagnostics)
        {
            this.Diagnostics = diagnostics;
            this.InvokeAsync(() => { this.StateHasChanged(); });

            _ = this.JS.SetMonacoDiagnosticsAsync(_editorId, diagnostics);
        }

        #endregion

        #region Commands

        private async Task SaveAsync()
        {
            this.FilterDescription.Code = await this.JS.GetMonacoValueAsync(_editorId);
            this.AppState.FilterSettings.AddOrUpdateFilterDescription(this.FilterDescription);
            this.ToasterService.ShowSuccess(message: "The filter has been saved.", icon: MatIconNames.Thumb_up);
        }

        #endregion
    }
}
