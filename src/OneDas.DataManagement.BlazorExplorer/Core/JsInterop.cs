using Microsoft.JSInterop;
using OneDas.DataManagement.BlazorExplorer.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneDas.DataManagement.BlazorExplorer.Core
{
    public static class JsInterop
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

        public static async Task UpdateChartAsync(IJSRuntime jsRuntime, AppStateViewModel appState, List<ChartEntry> chartEntries, DateTime begin, DateTime end, int count, double dt, bool beginAtZero)
        {
            var appStateRef = DotNetObjectReference.Create(appState);
            await jsRuntime.InvokeAsync<object>("UpdateChart", appStateRef, chartEntries, begin, end, count, dt, beginAtZero);
        }

        #endregion
    }
}
