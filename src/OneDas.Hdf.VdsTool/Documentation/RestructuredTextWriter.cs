using System;
using System.IO;
using System.Linq;

namespace OneDas.Hdf.VdsTool.Documentation
{
    public class RestructuredTextWriter
    {
        StreamWriter _streamWriter;

        public RestructuredTextWriter(StreamWriter streamWriter)
        {
            _streamWriter = streamWriter;
        }

        public void WriteHeading(string text, SectionHeader sectionHeader)
        {
            char character;
            bool overline;

            overline = false;

            switch (sectionHeader)
            {
                case SectionHeader.Part:
                    character = '#';
                    overline = true;
                    break;
                case SectionHeader.Chapter:
                    character = '*';
                    overline = true;
                    break;
                case SectionHeader.Section:
                    character = '=';
                    break;
                case SectionHeader.SubSection:
                    character = '-';
                    break;
                case SectionHeader.SubSubSection:
                    character = '^';
                    break;
                case SectionHeader.Paragraph:
                    character = '"';
                    break;
                default:
                    throw new NotImplementedException("sectionHeader");
            }

            if (overline)
            {
                _streamWriter.WriteLine(new string(character, text.Count()));
            }

            _streamWriter.WriteLine(text);
            _streamWriter.WriteLine(new string(character, text.Count()));
        }

        public void Write(string text)
        {
            _streamWriter.Write(text);
        }

        public void WriteTable(RestructuredTextTable table)
        {
            int[] maxWidthSet;

            maxWidthSet = new int[table.Header.Count];

            for (int i = 0; i < table.Header.Count; i++)
            {
                maxWidthSet[i] = table.Header[i].Count();

                if (table.RowSet.Any())
                {
                    maxWidthSet[i] = Math.Max(maxWidthSet[i], table.RowSet.Max(row => row[i].Count()));
                }
            }

            // table header [1/3]
            for (int i = 0; i < table.Header.Count; i++)
            {
                this.Write(new string('=', maxWidthSet[i]));
                this.Write(" ");
            }

            this.WriteLine();

            // table header [2/3]
            for (int i = 0; i < table.Header.Count; i++)
            {
                this.Write(table.Header[i].PadRight(maxWidthSet[i]));
                this.Write(" ");
            }

            this.WriteLine();

            // table header [3/3]
            for (int i = 0; i < table.Header.Count; i++)
            {
                this.Write(new string('=', maxWidthSet[i]));
                this.Write(" ");
            }

            this.WriteLine();

            // table content
            for (int i = 0; i < table.RowSet.Count(); i++)
            {
                for (int j = 0; j < table.Header.Count; j++)
                {
                    this.Write(table.RowSet[i][j].PadRight(maxWidthSet[j]));
                    this.Write(" ");
                }

                this.WriteLine();
            }

            // table footer
            for (int i = 0; i < table.Header.Count; i++)
            {
                this.Write(new string('=', maxWidthSet[i]));
                this.Write(" ");
            }
        }

        public void WriteNote(string text)
        {
            this.WriteLine($".. NOTE:: { text }");
        }

        public void WriteLine(string text = "")
        {
            _streamWriter.WriteLine(text);
        }
    }
}
