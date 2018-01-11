using System;
using System.IO;

namespace OneDas.Infrastructure
{
    public class OneDasOptions
    {
        public OneDasOptions()
        {
            this.BaseDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "OneDAS");
            this.NativeSampleRate = 100;
            this.ChunkPeriod = 60;
        }

        public string BaseDirectoryPath { get; set; }
        public uint NativeSampleRate { get; }
        public uint ChunkPeriod { get; }
    }
}
