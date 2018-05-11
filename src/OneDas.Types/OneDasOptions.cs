using System;
using System.IO;

namespace OneDas
{
    public class OneDasOptions
    {
        public OneDasOptions()
        {
            string baseDirectoryPath;

            baseDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OneDAS", "Core");

            this.BackupDirectoryPath = Path.Combine(baseDirectoryPath, "backup");
            this.ConfigurationDirectoryPath = Path.Combine(baseDirectoryPath, "config");
            this.DataDirectoryPath = Path.Combine(baseDirectoryPath, "data");
            this.NugetDirectoryPath = Path.Combine(baseDirectoryPath, "nuget");
            this.ProjectDirectoryPath = Path.Combine(baseDirectoryPath, "project");

            this.NugetProjectFilePath = Path.Combine(this.NugetDirectoryPath, "project.json");
            this.PackageTypeName = "OneDasExtension";
        }

        public string BackupDirectoryPath { get; set; }
        public string ConfigurationDirectoryPath { get; set; }
        public string DataDirectoryPath { get; set; }
        public string NugetDirectoryPath { get; set; }
        public string ProjectDirectoryPath { get; set; }

        public string NugetProjectFilePath { get; set; }
        public string PackageTypeName { get; set; }
    }
}
