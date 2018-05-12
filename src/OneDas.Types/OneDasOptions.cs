using System;
using System.IO;

namespace OneDas
{
    public class OneDasOptions
    {
        public OneDasOptions()
        {
            this.BaseDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OneDAS", "Core");
            this.PackageTypeName = "OneDasExtension";
            this.RestoreRuntimeId = "any";
        }

        public string BaseDirectoryPath { get; set; }
        public string PackageTypeName { get; set; }
        public string RestoreRuntimeId { get; set; }

        public string BackupDirectoryPath { get => Path.Combine(this.BaseDirectoryPath, "backup"); }
        public string ConfigurationDirectoryPath { get => Path.Combine(this.BaseDirectoryPath, "config"); }
        public string DataDirectoryPath { get => Path.Combine(this.BaseDirectoryPath, "data"); }
        public string NugetDirectoryPath { get => Path.Combine(this.BaseDirectoryPath, "nuget"); }
        public string ProjectDirectoryPath { get => Path.Combine(this.BaseDirectoryPath, "project"); }

        public string NugetProjectFilePath { get => Path.Combine(this.NugetDirectoryPath, "project.json"); }
    }
}