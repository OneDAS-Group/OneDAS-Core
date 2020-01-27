using System;
using System.IO;
using System.Linq;

namespace OneDas.Hdf.VdsTool.Commands
{
    public class InitCommand
    {
        public InitCommand()
        {
            if (!this.IsDirectoryEmpty(Environment.CurrentDirectory))
                throw new InvalidOperationException("The current folder is not empty.");
        }

        public void Run()
        {
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "DB_DATA"));
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "DB_META"));
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "DB_VDS"));
        }

        private bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }
    }
}
