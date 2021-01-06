using Microsoft.AspNetCore.Components;
using OneDas.DataManagement.Explorer.ViewModels;
using System;
using System.Collections.Generic;

namespace OneDas.DataManagement.Explorer.Shared
{
    public partial class CodeDefinitionSettingsBox
    {
        #region Properties

        [Parameter]
        public CodeDefinitionViewModel CodeDefinition { get; set; }

        [Parameter]
        public Action OnSave { get; set; }

        private bool CodeDefinitionProjectRequestDialogIsOpen { get; set; }

        #endregion

        #region Commands

        private void SelectProjectIds(List<string> projectIds)
        {
            this.CodeDefinition.RequestedProjectIds = projectIds;
        }

        public void OpenCodeDefinitionProjectRequestDialog()
        {
            this.CodeDefinitionProjectRequestDialogIsOpen = true;
        }

        #endregion

        #region Methods

        private void HandleValidSubmit()
        {
            this.OnSave?.Invoke();
        }

        #endregion
    }
}
