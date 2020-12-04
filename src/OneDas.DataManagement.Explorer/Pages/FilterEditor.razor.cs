using BlazorMonaco;
using BlazorMonaco.Bridge;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OneDas.DataManagement.Explorer.Core;
using OneDas.DataManagement.Explorer.Services;
using OneDas.DataManagement.Explorer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Explorer.Pages
{
    public partial class FilterEditor : IDisposable
    {
		#region Fields

		private bool _hasRegistered = false;
		private DotNetObjectReference<MonacoService> _objRef;
        private EventHandler _windowResizedHandler;

        #endregion

        #region Constructors

        public FilterEditor()
        {
            _windowResizedHandler = (sender, e) =>
            {
                // not yet fully working
                _editor?.Layout();
            };

            WindowResizeService.OnResize += _windowResizedHandler;

            this.PropertyChanged = (sender, e) =>
            {
                if (e.PropertyName == nameof(FilterSettingsViewModel.FilterDescriptions))
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
        protected IMatToaster Toaster { get; set; }

        [Inject]
        private IJSRuntime JS { get; set; }

        [Inject]
        private MonacoService MonacoService { get; set; }

        [Inject]
        private UserIdService UserIdService { get; set; }

		private List<MonacoService.Diagnostic> Diagnostics { get; set; }

        private FilterDescriptionViewModel FilterDescription { get; set; }

        #endregion

        #region Methods

        public void Dispose()
        {
            _objRef?.Dispose();
            WindowResizeService.OnResize -= _windowResizedHandler;
        }

        protected override void OnParametersSet()
        {
            this.CreateNewFilterDescription();
            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!_hasRegistered)
            {
                _objRef = DotNetObjectReference.Create(this.MonacoService);
                await JS.InvokeAsync<string>("registerProviders", _objRef);
                _hasRegistered = true;
            }
        }

        private async Task OnSaveAsync()
        {
            this.FilterDescription.Code = await _editor.GetValue();
            this.AppState.FilterSettings.AddOrUpdateFilterDescription(this.FilterDescription);

            this.Toaster.Add(
                "The filter has been saved.",
                MatToastType.Success,
                title: "Success!",
                icon: MatIconNames.Thumb_up);
        }

        private void SelectFilterDescription(FilterDescription filterDescription)
        {
            this.FilterDescription = new FilterDescriptionViewModel(filterDescription);
            _editor.SetValue(this.FilterDescription.Code);
        }

        private void CreateNewFilterDescription()
        {
            this.FilterDescription = new FilterDescriptionViewModel(new FilterDescription()
            {
                Name = "NewFilter",
                Group = this.UserIdService.User.Identity.Name,
                Unit = "-",
                SampleRate = this.AppState.SampleRateValues.First(),
                Code = this.MonacoService.DefaultCode
            });
        }

        private StandaloneEditorConstructionOptions EditorConstructionOptions(MonacoEditor editor)
        {
            return new StandaloneEditorConstructionOptions
            {
                AutomaticLayout = true,
                Language = "csharp",
                Value = this.MonacoService.DefaultCode,
                Theme = "vs-dark"
            };
        }

        private async Task OnDidChangeModelContent(ModelContentChangedEvent e)
        {
            var uri = (await _editor.GetModel()).Uri;
            var code = this.FilterDescription.Code;

            this.FilterDescription.Code = code;
            this.Diagnostics = await this.MonacoService.GetDiagnosticsAsync(code);

            await JS.InvokeAsync<string>("setDiagnostics", this.Diagnostics, uri);
            await this.InvokeAsync(() => this.StateHasChanged());
        }

        #endregion
    }
}
