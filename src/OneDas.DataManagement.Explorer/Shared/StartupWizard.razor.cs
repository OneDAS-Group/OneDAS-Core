using Microsoft.AspNetCore.Components;
using OneDas.DataManagement.Explorer.Core;
using OneDas.Types;

namespace OneDas.DataManagement.Explorer.Shared
{
    public partial class StartupWizard
    {
        [Inject]
        public OneDasExplorerOptions Options { get; set; }

        [Inject]
        public StateManager StateManager { get; set; }

        public string Error { get; set; }

        public void TryRunApp()
        {
            if (!this.StateManager.TryRunApp(out var exception))
            {
                this.Error = exception.GetFullMessage();
            }
        }
    }
}
