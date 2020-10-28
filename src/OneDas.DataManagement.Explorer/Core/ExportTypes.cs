using OneDas.DataManagement.Infrastructure;
using OneDas.Extension.Csv;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;

namespace OneDas.DataManagement.Explorer.Core
{
    public record ExportParameters
    {
        public DateTime Begin { get; set; } = DateTime.UtcNow.Date.AddDays(-2);
        public DateTime End { get; set; } = DateTime.UtcNow.Date.AddDays(-1);
        public FileGranularity FileGranularity { get; set; } = FileGranularity.Hour;
        public FileFormat FileFormat { get; set; } = FileFormat.CSV;
        public List<string> ChannelPaths { get; set; } = new List<string>();

        // special parameters
        public uint CsvSignificantFigures { get; set; } = 4;
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
