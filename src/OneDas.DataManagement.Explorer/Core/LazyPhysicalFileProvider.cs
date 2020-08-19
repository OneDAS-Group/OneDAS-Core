using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System.IO;

namespace OneDas.DataManagement.Explorer.Core
{
    public class LazyPhysicalFileProvider : IFileProvider
    {
        private string _relativePath;
        private OneDasExplorerOptions _options;
        private PhysicalFileProvider _physicalFileProvider;

        public LazyPhysicalFileProvider(OneDasExplorerOptions options, string relativePath)
        {
            _options = options;
            _relativePath = relativePath;
        }

        private PhysicalFileProvider GetPhysicalFileProvider()
        {
            var path = Path.Combine(_options.DataBaseFolderPath, _relativePath);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (_physicalFileProvider == null || _physicalFileProvider.Root != path)
                _physicalFileProvider = new PhysicalFileProvider(path);

            return _physicalFileProvider;
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return GetPhysicalFileProvider().GetDirectoryContents(subpath);
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            return GetPhysicalFileProvider().GetFileInfo(subpath);
        }

        public IChangeToken Watch(string filter)
        {
            return GetPhysicalFileProvider().Watch(filter);
        }
    }
}
