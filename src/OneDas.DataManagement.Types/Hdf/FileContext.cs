using System;

namespace OneDas.DataManagement.Hdf
{
    public class FileContext
    {
        public FileContext(int formatVersion, DateTime dateTime, string filePath)
        {
            this.FormatVersion = formatVersion;
            this.DateTime = dateTime;
            this.FilePath = filePath;
        }

        public int FormatVersion { get; private set; }
        public DateTime DateTime { get; private set; }
        public string FilePath { get; private set; }
    }
}
