using Microsoft.JSInterop;
using OneDas.DataManagement.Explorer.Services;
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
            await jsRuntime.InvokeVoidAsync("FileSaveAs", fileName, href);
        }

        public static ValueTask<int> GetBrowserTimeZoneOffset(this IJSRuntime jsRuntime, DateTime value)
        {
            return jsRuntime.InvokeAsync<int>("GetBrowserTimeZoneOffset", value);
        }

        public static async Task UpdateChartAsync(this IJSRuntime jsRuntime, UserStateViewModel userState, List<ChartEntry> chartEntries, DateTime begin, DateTime end, int count, double dt, bool beginAtZero)
        {
            var userStateRef = DotNetObjectReference.Create(userState);
            await jsRuntime.InvokeVoidAsync("UpdateChart", userStateRef, chartEntries, begin, end, count, dt, beginAtZero);
        }

        public static async Task CreateMonacoEditorAsync(this IJSRuntime jsRuntime, string editorId, Dictionary<string, object> options)
        {
            await jsRuntime.InvokeVoidAsync("CreateMonacoEditor", editorId, options);
        }

        public static async Task RegisterMonacoProvidersAsync(this IJSRuntime jsRuntime, string editorId, DotNetObjectReference<MonacoService> dotnetHelper)
        {
            await jsRuntime.InvokeVoidAsync("RegisterMonacoProviders", editorId, dotnetHelper);
        }

        public static async Task SetMonacoValueAsync(this IJSRuntime jsRuntime, string editorId, string value)
        {
            await jsRuntime.InvokeVoidAsync("SetMonacoValue", editorId, value);
        }

        public static ValueTask<string> GetMonacoValueAsync(this IJSRuntime jsRuntime, string editorId)
        {
            return jsRuntime.InvokeAsync<string>("GetMonacoValue", editorId);
        }

        #endregion
    }
}
