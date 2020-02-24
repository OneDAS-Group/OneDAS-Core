using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net.Mime;
using System.Text.RegularExpressions;

namespace OneDas.DataManagement.Explorer.Web.Pages
{
    public class FileModel : PageModel
    {
        private OneDasExplorerOptions _options;

        public FileModel(IOptions<OneDasExplorerOptions> options)
        {
            _options = options.Value;
        }

        public FileStreamResult OnGet(string fileName)
        {
            string basePath;
            string filePath;
            string realFileName;

            basePath = Path.Combine(_options.SupportDirectoryPath, "EXPORT");
            filePath = Path.GetFullPath(Path.Combine(basePath, fileName));

            realFileName = Path.GetFileName(filePath);
            realFileName = Regex.Replace(realFileName, "(_[0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12})", string.Empty);

            if (filePath.StartsWith(basePath))
            {
                if (System.IO.File.Exists(filePath))
                {
                    return this.File(new FileStream(filePath, FileMode.Open, FileAccess.Read), MediaTypeNames.Application.Octet, realFileName);
                }
                else
                {
                    throw new Exception($"The requested file '{ fileName }' could not be found.");
                }
            }
            else
            {
                throw new Exception($"The path of requested file '{ fileName }' does not point to the download folder.");
            }
        }
    }
}
