using BlazorMonaco;
using BlazorMonaco.Bridge;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OneDas.DataManagement.Explorer.Core;
using OneDas.DataManagement.Explorer.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Explorer.Pages
{
    public partial class Playground : IDisposable
    {
		#region Fields

		private bool _hasRegistered = false;
		private DotNetObjectReference<MonacoService> _objRef;
        private EventHandler _windowResizedHandler { get; set; }

        #endregion

        #region Constructors

        public Playground()
        {
            _windowResizedHandler = (sender, e) =>
            {
                // not yet fully working
                _editor?.Layout();
            };
            WindowResizeService.OnResize += _windowResizedHandler;
        }

        #endregion

        #region Properties

        [Inject]
        private IJSRuntime JS { get; set; }

        [Inject]
        private MonacoService MonacoService { get; set; }

		private List<MonacoService.Diagnostic> Diagnostics { get; set; }

        #endregion

        #region Methods

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
            var code = await _editor.GetValue();

            this.Diagnostics = await this.MonacoService.GetDiagnosticsAsync(code);

            await JS.InvokeAsync<string>("setDiagnostics", this.Diagnostics, uri);
            await this.InvokeAsync(() => this.StateHasChanged());
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

        public void Dispose()
        {
            _objRef?.Dispose();
            WindowResizeService.OnResize -= _windowResizedHandler;
        }

        #endregion
    }
}
