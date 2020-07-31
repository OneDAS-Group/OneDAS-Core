using OneDas.Extension.Csv;

namespace OneDas.DataManagement.Explorer.Core
{
    public class ExtendedExportConfiguration
    {
        #region Constructors

        public ExtendedExportConfiguration()
        {
            this.CsvSignificantFigures = 4;
            this.CsvRowIndexFormat = CsvRowIndexFormat.Index;
        }

        #endregion

        #region Properties

        public uint CsvSignificantFigures { get; set; }

        public CsvRowIndexFormat CsvRowIndexFormat { get; set; }

        #endregion
    }
}
