using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace OneDas.DataManagement.BlazorExplorer.Core
{
    public static class FileUtilities
    {
        public async static Task BlobSaveAs(IJSRuntime jsRuntime, string fileName, byte[] data)
        {
            await jsRuntime.InvokeAsync<object>("BlobSaveAs", fileName, Convert.ToBase64String(data));
        }

        public async static Task FileSaveAs(IJSRuntime jsRuntime, string fileName, string href)
        {
            await jsRuntime.InvokeAsync<object>("FileSaveAs", fileName, href);
        }
    }
}
