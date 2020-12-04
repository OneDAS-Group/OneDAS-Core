using Microsoft.AspNetCore.Components;
using OneDas.DataManagement.Explorer.ViewModels;
using System;
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

        #endregion

        #region Methods

        private void HandleValidSubmit()
        {
            this.OnSaveAsync?.Invoke();
        }

        #endregion
    }
}
