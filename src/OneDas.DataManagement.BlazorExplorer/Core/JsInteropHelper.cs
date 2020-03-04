using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneDas.DataManagement.BlazorExplorer.Core
{
    public static class JsInteropHelper
    {
        #region Methods

        public static async Task BlobSaveAs(IJSRuntime jsRuntime, string fileName, byte[] data)
        {
            await jsRuntime.InvokeAsync<object>("BlobSaveAs", fileName, Convert.ToBase64String(data));
        }

        public static async Task FileSaveAs(IJSRuntime jsRuntime, string fileName, string href)
        {
            await jsRuntime.InvokeAsync<object>("FileSaveAs", fileName, href);
        }

        public static async Task DrawChartAsync(IJSRuntime jsRuntime, List<ChartEntry> chartEntries)
        {
            await jsRuntime.InvokeAsync<object>("DrawChart", chartEntries);
        }

        public static async Task UpdateChartDataAsync(IJSRuntime jsRuntime)
        {
            await jsRuntime.InvokeAsync<object>("UpdateChartData");
        }

        #endregion
    }
}
