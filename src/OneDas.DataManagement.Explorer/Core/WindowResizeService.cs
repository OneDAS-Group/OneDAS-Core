using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Explorer.Core
{
    public class WindowResizeService
    {
        public static event EventHandler OnResize;

        [JSInvokable]
        public static Task RaiseOnWindowResizeAsync()
        {
            WindowResizeService.OnResize?.Invoke(null, EventArgs.Empty);
            return Task.CompletedTask;
        }
    }
}
