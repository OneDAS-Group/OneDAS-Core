using System;

namespace OneDas.DataManagement.Hdf
{
    public class SourceFileInfo
    {
        #region "Constructors"

        public SourceFileInfo(string filePath, ulong length, DateTime startDateTime)
        {
            this.FilePath = filePath;
            this.Length = length;
            this.StartDateTime = startDateTime;
        }

        #endregion

        #region "Properties"

        public string FilePath { get; }
        public ulong Length { get; }
        public DateTime StartDateTime { get; }

        #endregion
    }
}