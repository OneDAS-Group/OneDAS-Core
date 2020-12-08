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
		private DotNetObjectReference<MonacoService> _objRef;

        #endregion

        #region Properties

        public List<Diagnostic> Diagnostics { get; set; }

        protected PropertyChangedEventHandler FilterSettingsPropertyChangedHandler { get; set; }

        protected EventHandler<List<Diagnostic>> DiagnosticsUpdatedHandler { get; set; }

        [Inject]
        private ToasterService ToasterService { get; set; }

        [Inject]
        private IJSRuntime JS { get; set; }

        [Inject]
        private MonacoService MonacoService { get; set; }

        [Inject]
        private UserIdService UserIdService { get; set; }

        private FilterDescriptionViewModel FilterDescription { get; set; }

        #endregion

        #region Methods

        public void Dispose()
        {
            _objRef?.Dispose();
            this.AppState.FilterSettings.PropertyChanged -= this.FilterSettingsPropertyChangedHandler;
            this.MonacoService.DiagnosticsUpdated -= this.DiagnosticsUpdatedHandler;
        }

        protected override void OnParametersSet()
        {
            this.CreateNewFilterDescription();
            base.OnParametersSet();

            // filter settings changed
            this.FilterSettingsPropertyChangedHandler = (sender, e) =>
            {
                if (e.PropertyName == nameof(FilterSettingsViewModel.FilterDescriptions))
                {
                    this.InvokeAsync(() => { this.StateHasChanged(); });
                }
            };

            this.AppState.FilterSettings.PropertyChanged += this.FilterSettingsPropertyChangedHandler;

            // diagnostics updates
            this.DiagnosticsUpdatedHandler = (sender, e) =>
            {
                this.Diagnostics = e;
                this.InvokeAsync(() => { this.StateHasChanged(); });
            };

            this.MonacoService.DiagnosticsUpdated += this.DiagnosticsUpdatedHandler;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _objRef = DotNetObjectReference.Create(this.MonacoService);
                _editorId = "1";

                await JS.CreateMonacoEditorAsync(_editorId, this.MonacoEditorOptions);
                await JS.RegisterMonacoProvidersAsync(_editorId, _objRef);
            }
        }

        private async Task OnSaveAsync()
        {
            this.FilterDescription.Code = await JS.GetMonacoValueAsync(_editorId);
            this.AppState.FilterSettings.AddOrUpdateFilterDescription(this.FilterDescription);
            this.ToasterService.ShowSuccess(message: "The filter has been saved.", icon: MatIconNames.Thumb_up);
        }

        private void SelectFilterDescription(FilterDescription filterDescription)
        {
            this.FilterDescription = new FilterDescriptionViewModel(filterDescription);
            JS.SetMonacoValueAsync(_editorId, this.FilterDescription.Code);
        }

        private void CreateNewFilterDescription()
        {
            this.FilterDescription = new FilterDescriptionViewModel(new FilterDescription()
            {
                Name = "NewFilter",
                Group = this.UserIdService.User.Identity.Name,
                Unit = "-",
                SampleRate = this.UserState.SampleRateValues.FirstOrDefault(),
                Code = this.MonacoService.DefaultCode
            });
        }

        private Dictionary<string, object> MonacoEditorOptions
        {
            get
            {
                return new Dictionary<string, object>
                {
                    ["automaticLayout"] = true,
                    ["language"] = "csharp",
                    ["scrollBeyondLastLine"] = false,
                    ["value"] = this.MonacoService.DefaultCode,
                    ["theme"] = "vs-dark"
                };
            }
        }

        #endregion
    }
}
