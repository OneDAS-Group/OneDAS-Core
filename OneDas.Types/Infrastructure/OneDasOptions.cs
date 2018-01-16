using System;
using System.IO;

namespace OneDas.Infrastructure
{
    public class OneDasOptions
    {
        public OneDasOptions()
        {
            this.NativeSampleRate = 100;
            this.ChunkPeriod = 60;
            this.ProductName = "OneDAS";

            this.BaseDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), this.ProductName);
        }

        public uint NativeSampleRate { get; }

        public uint ChunkPeriod { get; }

        public string ProductName { get; }

        public string BaseDirectoryPath { get; set; }
    }
}
