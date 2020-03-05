using Microsoft.AspNetCore.Components;
using OneDas.DataManagement.BlazorExplorer.ViewModels;
using System.IO;

namespace OneDas.DataManagement.BlazorExplorer.Shared
{
    public partial class Presets
    {
        #region Properties

        [Inject]
        public AppStateViewModel AppState { get; set; }

        [Parameter]
        public bool IsOpen { get; set; }

        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

        #endregion

        #region Methods

        public string GetFileName(string filePath)
        {
            return Path.GetFileName(filePath);
        }

        public string GetHref(string filePath)
        {
            return $"/download/{filePath}/{this.GetFileName(filePath)}";
        }

        private void OnIsOpenChanged(bool value)
        {
            this.IsOpen = value;
            this.IsOpenChanged.InvokeAsync(value);
        }

        #endregion
    }
}
