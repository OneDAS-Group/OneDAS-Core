using Microsoft.AspNetCore.Components;
using OneDas.DataManagement.Explorer.Core;
using System;

namespace OneDas.DataManagement.Explorer.Shared
{
    public partial class CodeDefinitionCreateModal
    {
        #region Properties

        [Parameter]
        public bool IsOpen { get; set; }

        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

        [Parameter]
        public Action<CodeType> OnCodeTypeSelected { get; set; }

        #endregion

        #region Methods

        private void Accept(CodeType codeType)
        {
            this.OnIsOpenChanged(false);
            this.OnCodeTypeSelected?.Invoke(codeType);
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
