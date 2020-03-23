using Microsoft.AspNetCore.Components;
using OneDas.DataManagement.Explorer.ViewModels;

namespace OneDas.DataManagement.Explorer.Shared
{
    public partial class SettingsContentBox
    {
        #region Properties

        [Inject]
        public SettingsViewModel Settings { get; set; }

        #endregion
    }
}
