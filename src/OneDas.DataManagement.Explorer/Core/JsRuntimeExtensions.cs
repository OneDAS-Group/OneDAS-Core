using Microsoft.JSInterop;
using OneDas.DataManagement.Explorer.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Explorer.Core
{
    internal static class JsRuntimeExtensions
    {
        #region Methods

        public static async Task BlobSaveAs(this IJSRuntime jsRuntime, string fileName, byte[] data)
        {
            await jsRuntime.InvokeAsync<object>("BlobSaveAs", fileName, Convert.ToBase64String(data));
        }

        public static async Task FileSaveAs(this IJSRuntime jsRuntime, string fileName, string href)
        {
            await jsRuntime.InvokeAsync<object>("FileSaveAs", fileName, href);
        }

        public static ValueTask<int> GetBrowserTimeZoneOffset(this IJSRuntime jsRuntime, DateTime value)
        {
            return jsRuntime.InvokeAsync<int>("GetBrowserTimeZoneOffset", value);
        }

        public static async Task UpdateChartAsync(this IJSRuntime jsRuntime, AppStateViewModel appState, List<ChartEntry> chartEntries, DateTime begin, DateTime end, int count, double dt, bool beginAtZero)
        {
            var appStateRef = DotNetObjectReference.Create(appState);
            await jsRuntime.InvokeAsync<object>("UpdateChart", appStateRef, chartEntries, begin, end, count, dt, beginAtZero);
        }

        #endregion
    }
}
