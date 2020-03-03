using OneDas.Infrastructure;
using System;
using System.Collections.Generic;

namespace OneDas.DataManagement.BlazorExplorer.Core
{
    public class ExportConfiguration
    {
        #region Constructors

        public ExportConfiguration()
        {
            this.FormatVersion = 1;
            this.DateTimeBegin = DateTime.UtcNow.Date.AddDays(-2);
            this.DateTimeEnd = DateTime.UtcNow.Date.AddDays(-1);
            this.FileGranularity = FileGranularity.Hour;
            this.FileFormat = FileFormat.CSV;
            this.Variables = new List<string>();
        }

        #endregion

        #region Properties

        public int FormatVersion { get; set; }

        public DateTime DateTimeBegin { get; set; }

        public DateTime DateTimeEnd { get; set; }

        public FileGranularity FileGranularity { get; set; }

        public FileFormat FileFormat { get; set; }

        public string SampleRate { get; set; }

        public List<string> Variables { get; set; }

        #endregion
    }
}
