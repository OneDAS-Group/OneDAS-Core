using Microsoft.AspNetCore.Components;
using System;

namespace OneDas.DataManagement.Explorer.Shared
{
    public partial class CodeDefinitionDeleteModal
    {
        #region Properties

        [Parameter]
        public bool IsOpen { get; set; }

        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

        [Parameter]
        public Action OnDelete { get; set; }

        #endregion

        #region Methods

        private void Accept()
        {
            this.OnIsOpenChanged(false);
            this.OnDelete?.Invoke();
        }

        private void Cancel()
        {
            this.OnIsOpenChanged(false);
        }

        private void OnIsOpenChanged(bool value)
        {
            this.IsOpen = value;
            this.IsOpenChanged.InvokeAsync(value);
        }

        #endregion
    }
}
