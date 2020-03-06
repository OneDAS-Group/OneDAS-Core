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
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "DATA"));
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
