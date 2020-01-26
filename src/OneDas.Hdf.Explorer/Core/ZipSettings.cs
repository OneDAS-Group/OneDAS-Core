using OneDas.Database;
using OneDas.DataManagement.Extensibility;
using System;

namespace OneDas.Hdf.Explorer.Core
{
    public class ZipSettings
    {
        public ZipSettings(DateTime dateTimeBegin, CampaignInfo campaign, DataReaderExtensionBase dataReader, double sampleRate, ulong start, ulong block, ulong segmentLength)
        {
            this.DateTimeBegin = dateTimeBegin;
            this.Campaign = campaign;
            this.DataReader = dataReader;
            this.SampleRate = sampleRate;
            this.Start = start;
            this.Block = block;
            this.SegmentLength = segmentLength;
        }

        public DateTime DateTimeBegin { get; private set; }
        public CampaignInfo Campaign { get; private set; }
        public DataReaderExtensionBase DataReader { get; private set; }
        public double SampleRate { get; private set; }
        public ulong Start { get; private set; }
        public ulong Block { get; private set; }
        public ulong SegmentLength { get; private set; }
    }
}
