using Microsoft.AspNetCore.Components;
using OneDas.DataManagement.Explorer.ViewModels;

namespace OneDas.DataManagement.Explorer.Shared
{
    public partial class LogBook
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

        private void OnIsOpenChanged(bool value)
        {
            this.IsOpen = value;
            this.IsOpenChanged.InvokeAsync(value);
        }

        #endregion
    }
}
