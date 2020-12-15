using Microsoft.AspNetCore.Components;
using OneDas.DataManagement.Explorer.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Explorer.Shared
{
    public partial class FilterDescriptionSettingsBox
    {
        #region Properties

        [Parameter]
        public FilterDescriptionViewModel FilterDescription { get; set; }

        [Parameter]
        public Func<Task> OnSaveAsync { get; set; }

        private bool FilterProjectRequestDialogIsOpen { get; set; }

        #endregion

        #region Commands

        private void SelectProjectIds(List<string> projectIds)
        {
            this.FilterDescription.RequestedProjectIds = projectIds;
        }

        public void OpenFilterProjectRequestDialog()
        {
            this.FilterProjectRequestDialogIsOpen = true;
        }

        #endregion

        #region Methods

        private void HandleValidSubmit()
        {
            this.OnSaveAsync?.Invoke();
        }

        #endregion
    }
}
