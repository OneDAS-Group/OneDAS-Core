using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace OneDas.Hdf.VdsTool.Documentation
{
    public class RestructuredTextTable
    {
        private List<List<string>> _rowSet;

        public RestructuredTextTable(List<string> header)
        {
            Contract.Requires(header != null);

            _rowSet = new List<List<string>>();

            this.Header = header;
        }

        public void AddRow(List<string> row)
        {
            Contract.Requires(row != null);

            if (row.Count != this.Header.Count)
            {
                throw new ArgumentException(nameof(row));
            }

            _rowSet.Add(row);
        }

        public List<string> Header { get; private set; }

        public IReadOnlyList<List<string>> RowSet
        {
            get
            {
                return (IReadOnlyList<List<string>>)_rowSet;
            }
        }
    }
}
