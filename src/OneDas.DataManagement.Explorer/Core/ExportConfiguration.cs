using OneDas.DataManagement.Infrastructure;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;

namespace OneDas.DataManagement.Explorer.Core
{
    public class ExportConfiguration
    {
        #region Constructors

        public ExportConfiguration()
        {
            this.FormatVersion = 2;
            this.Begin = DateTime.UtcNow.Date.AddDays(-2);
            this.End = DateTime.UtcNow.Date.AddDays(-1);
            this.FileGranularity = FileGranularity.Hour;
            this.FileFormat = FileFormat.CSV;
            this.Channels = new List<string>();
            this.Extended = new ExtendedExportConfiguration();
        }

        #endregion

        #region Properties

        public int FormatVersion { get; set; }

        public DateTime Begin { get; set; }

        public DateTime End { get; set; }

        public FileGranularity FileGranularity { get; set; }

        public FileFormat FileFormat { get; set; }

        public string SampleRate { get; set; }

        public List<string> Channels { get; set; }

        public ExtendedExportConfiguration Extended { get; set; }

        #endregion

        #region Methods

        public static ExportConfiguration UpdateVersion(ExportConfiguration config)
        {
            if (config.FormatVersion == 1)
            {
                config.Extended = new ExtendedExportConfiguration();
                config.FormatVersion = 2;
            }

            return config;
        }

        #endregion
    }
}
