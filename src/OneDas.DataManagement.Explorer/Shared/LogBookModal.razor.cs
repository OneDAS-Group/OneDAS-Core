using Microsoft.AspNetCore.Components;
using OneDas.DataManagement.Explorer.Core;

namespace OneDas.DataManagement.Explorer.Shared
{
    public partial class LogBookModal
    {
        #region Properties

        [Inject]
        public UserState UserState { get; set; }

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
