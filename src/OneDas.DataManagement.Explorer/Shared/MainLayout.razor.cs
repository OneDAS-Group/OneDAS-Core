using Microsoft.AspNetCore.Components;
using OneDas.DataManagement.Explorer.Core;

namespace OneDas.DataManagement.Explorer.Shared
{
    public partial class MainLayout
    {
        [Inject]
        public OneDasExplorerOptions Options { get; set; }
    }
}
