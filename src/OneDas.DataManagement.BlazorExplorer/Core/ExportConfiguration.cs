using OneDas.Infrastructure;
using System;
using System.Collections.Generic;

namespace OneDas.DataManagement.BlazorExplorer.Core
{
    public class ExportConfiguration
    {
        #region Fields

        internal DateTime DateTimeBeginField;
        internal DateTime DateTimeEndField;
        internal FileGranularity FileGranularityField;
        internal FileFormat FileFormatField;
        internal string SampleRateField;
        internal List<string> VariablesField;

        #endregion

        #region Constructors

        public ExportConfiguration()
        {
            this.DateTimeBegin = DateTime.UtcNow.Date.AddDays(-2);
            this.DateTimeEnd = DateTime.UtcNow.Date.AddDays(-1);
            this.FileGranularity = FileGranularity.Hour;
            this.FileFormat = FileFormat.CSV;
            this.Variables = new List<string>();
        }

        #endregion

        #region Properties

        public DateTime DateTimeBegin
        {
            get { return this.DateTimeBeginField; }
            set { this.DateTimeBeginField = value; }
        }

        public DateTime DateTimeEnd
        {
            get { return this.DateTimeEndField; }
            set { this.DateTimeEndField = value; }
        }

        public FileGranularity FileGranularity
        {
            get { return this.FileGranularityField; }
            set { this.FileGranularityField = value; }
        }

        public FileFormat FileFormat
        {
            get { return this.FileFormatField; }
            set { this.FileFormatField = value; }
        }

        public string SampleRate
        {
            get { return this.SampleRateField; }
            set { this.SampleRateField = value; }
        }

        public List<string> Variables
        {
            get { return this.VariablesField; }
            set { this.VariablesField = value; }
        }

        #endregion
    }
}
