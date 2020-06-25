using System;
using System.IO;
using System.Linq;

namespace OneDas.DataManagement.Explorer.Core
{
    public class DBInitializer
    {
        public void Initialize()
        {
            if (!this.IsDirectoryEmpty(Environment.CurrentDirectory))
                throw new InvalidOperationException("The current folder is not empty.");

            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "ATTACHMENTS"));
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "DATA"));
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "EXPORT"));
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "EXTENSION"));
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "META"));
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "PRESETS"));
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "VDS"));
        }

        private bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }
    }
}
