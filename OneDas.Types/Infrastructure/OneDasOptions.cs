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

            this.DataDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OneDAS", "data");
        }

        public uint NativeSampleRate { get; }

        public uint ChunkPeriod { get; }

        public string DataDirectoryPath { get; set; }
    }
}
