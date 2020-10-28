using OneDas.DataManagement.Infrastructure;
using OneDas.Extension.Csv;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;

namespace OneDas.DataManagement.Explorer.Core
{
    public record ExportParameters
    {
        /// <example>2020-10-01T16:30:00Z</example>
        public DateTime Begin { get; set; } = DateTime.UtcNow.Date.AddDays(-2);

        /// <example>2020-10-02T19:00:00Z</example>
        public DateTime End { get; set; } = DateTime.UtcNow.Date.AddDays(-1);

        /// <example>Hour</example>
        public FileGranularity FileGranularity { get; set; } = FileGranularity.Hour;

        /// <example>CSV</example>
        public FileFormat FileFormat { get; set; } = FileFormat.CSV;

        /// <example>["/IN_MEMORY/ALLOWED/TEST/T1/1 s_mean", "/IN_MEMORY/ALLOWED/TEST/V1/1 s_mean"]</example>
        public List<string> ChannelPaths { get; set; } = new List<string>();

        /// <example>4</example>
        public uint CsvSignificantFigures { get; set; } = 4;

        /// <example>Index</example>
        public CsvRowIndexFormat CsvRowIndexFormat { get; set; } = CsvRowIndexFormat.Index;
    }

    public static class ExportParametersExtensions
    {
        public static ExportParameters UpdateVersion(this ExportParameters parameters)
        {
            // here we could adapt old parameter dictionary names or initialize fields that have been added later
            return parameters;
        }
    }
}
