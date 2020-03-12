using Microsoft.AspNetCore.Components;
using OneDas.DataManagement.BlazorExplorer.ViewModels;

namespace OneDas.DataManagement.BlazorExplorer.Shared
{
    public partial class SettingsContentBox
    {
        #region Properties

        [Inject]
        public SettingsViewModel Settings { get; set; }

        #endregion
    }
}
