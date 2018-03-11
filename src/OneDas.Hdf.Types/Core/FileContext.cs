using System;

namespace OneDas.Hdf.Core
{
    public class FileContext
    {
        public FileContext(long fileId, int formatVersion, DateTime dateTime, string filePath)
        {
            this.FileId = fileId;
            this.FormatVersion = formatVersion;
            this.DateTime = dateTime;
            this.FilePath = filePath;
        }

        public long FileId { get; private set; }
        public int FormatVersion { get; private set; }
        public DateTime DateTime { get; private set; }
        public string FilePath { get; private set; }
    }
}
