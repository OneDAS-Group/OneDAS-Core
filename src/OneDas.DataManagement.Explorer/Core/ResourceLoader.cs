using System;
using System.IO;
using System.Reflection;

namespace OneDas.DataManagement.Explorer.Core
{
    public static class ResourceLoader
    {
        public static Stream GetResourceStream(string name)
        {
            var assembly = typeof(ResourceLoader).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream(name);

            if (stream == null)
            {
                throw new InvalidOperationException($"Resource '{name}' not found in {assembly.FullName}.");
            }

            return stream;
        }

        public static byte[] GetResourceBlob(string name)
        {
            using (var stream = GetResourceStream(name))
            {
                var bytes = new byte[stream.Length];

                using (var memoryStream = new MemoryStream(bytes))
                {
                    stream.CopyTo(memoryStream);
                }

                return bytes;
            }
        }

        public static byte[] GetOrCreateResource(ref byte[]? resource, string name)
        {
            if (resource == null)
                resource = GetResourceBlob(name);

            return resource;
        }
    }
}
