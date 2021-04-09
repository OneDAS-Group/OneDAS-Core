using Microsoft.JSInterop;
using OneDas.DataManagement.Explorer.Pages;
using OneDas.DataManagement.Explorer.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static OneDas.DataManagement.Explorer.Services.MonacoService;

namespace OneDas.DataManagement.Explorer.Core
{
    internal static class JsRuntimeExtensions
    {
        #region Methods

        public static ValueTask WriteToClipboard(this IJSRuntime jsRuntime, string text)
        {
            return jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
        }

        public static ValueTask BlobSaveAs(this IJSRuntime jsRuntime, string fileName, byte[] data)
        {
            return jsRuntime.InvokeVoidAsync("BlobSaveAs", fileName, Convert.ToBase64String(data));
        }

        public static ValueTask FileSaveAs(this IJSRuntime jsRuntime, string fileName, string href)
        {
            return jsRuntime.InvokeVoidAsync("FileSaveAs", fileName, href);
        }

        public static ValueTask<int> GetBrowserTimeZoneOffset(this IJSRuntime jsRuntime, DateTime value)
        {
            return jsRuntime.InvokeAsync<int>("GetBrowserTimeZoneOffset", value);
        }

        public static ValueTask UpdateChartAsync(this IJSRuntime jsRuntime, UserState userState, List<ChartEntry> chartEntries, DateTime begin, DateTime end, int count, double dt, bool beginAtZero)
        {
            var userStateRef = DotNetObjectReference.Create(userState);
            return jsRuntime.InvokeVoidAsync("UpdateChart", userStateRef, chartEntries, begin, end, count, dt, beginAtZero);
        }

        public static ValueTask CreateMonacoEditorAsync(this IJSRuntime jsRuntime, string editorId, Dictionary<string, object> options)
        {
            return jsRuntime.InvokeVoidAsync("CreateMonacoEditor", editorId, options);
        }

        public static ValueTask RegisterMonacoProvidersAsync(this IJSRuntime jsRuntime, string editorId, DotNetObjectReference<FilterEditor> filterEditor, DotNetObjectReference<MonacoService> monacoService)
        {
            return jsRuntime.InvokeVoidAsync("RegisterMonacoProviders", editorId, filterEditor, monacoService);
        }

        public static ValueTask SetMonacoValueAsync(this IJSRuntime jsRuntime, string editorId, string value)
        {
            return jsRuntime.InvokeVoidAsync("SetMonacoValue", editorId, value);
        }

        public static ValueTask<string> GetMonacoValueAsync(this IJSRuntime jsRuntime, string editorId)
        {
            return jsRuntime.InvokeAsync<string>("GetMonacoValue", editorId);
        }

        public static ValueTask SetMonacoDiagnosticsAsync(this IJSRuntime jsRuntime, string editorId, List<Diagnostic> diagnostics)
        {
            return jsRuntime.InvokeVoidAsync("SetMonacoDiagnostics", editorId, diagnostics);
        }

        #endregion
    }
}
