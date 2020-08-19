using Microsoft.AspNetCore.Components;
using OneDas.DataManagement.Explorer.Core;
using OneDas.DataManagement.Explorer.ViewModels;

namespace OneDas.DataManagement.Explorer.Shared
{
    public partial class SettingsContentBox
    {
        #region Properties

        [Inject]
        public SettingsViewModel Settings { get; set; }

        [Inject]
        public OneDasExplorerOptions Options { get; set; }

        #endregion

        #region Methods

        public void SaveOptions()
        {
            this.Options.Save(Program.OptionsFilePath);
        }

        #endregion
    }
}
