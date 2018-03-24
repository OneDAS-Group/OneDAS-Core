using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net.Mime;

namespace OneDas.Hdf.Explorer.Web.Pages
{
    public class FileModel : PageModel
    {
        private HdfExplorerOptions _options;

        public FileModel(IOptions<HdfExplorerOptions> options)
        {
            _options = options.Value;
        }

        public FileStreamResult OnGet(string fileName)
        {
            string basePath = Path.Combine(_options.SupportDirectoryPath, "EXPORT");
            string filePath = Path.GetFullPath(Path.Combine(basePath, fileName));

            if (filePath.StartsWith(basePath))
            {
                if (System.IO.File.Exists(filePath))
                {
                    return this.File(new FileStream(filePath, FileMode.Open, FileAccess.Read), MediaTypeNames.Application.Octet, Path.GetFileName(filePath));
                }
                else
                {
                    throw new Exception($"The requested file '{ fileName }' could not be found.");
                }
            }
            else
            {
                throw new Exception($"The requested file '{ fileName }' is not within the downloads folder.");
            }
        }
    }
}
