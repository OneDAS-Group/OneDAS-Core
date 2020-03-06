using OneDas.DataManagement.Database;
using OneDas.DataManagement.Extensibility;
using System;

namespace OneDas.DataManagement.BlazorExplorer.Core
{
    public class ZipSettings
    {
        public ZipSettings(DateTime dateTimeBegin,
                           CampaignInfo campaign,
                           DataReaderExtensionBase nativeDataReader,
                           DataReaderExtensionBase aggregationDataReader,
                           double sampleRate,
                           ulong start,
                           ulong block,
                           ulong segmentLength)
        {
            this.DateTimeBegin = dateTimeBegin;
            this.Campaign = campaign;
            this.NativeDataReader = nativeDataReader;
            this.AggregationDataReader = aggregationDataReader;
            this.SampleRate = sampleRate;
            this.Start = start;
            this.Block = block;
            this.SegmentLength = segmentLength;
        }

        public DateTime DateTimeBegin { get; private set; }

        public CampaignInfo Campaign { get; private set; }

        public DataReaderExtensionBase NativeDataReader { get; private set; }

        public DataReaderExtensionBase AggregationDataReader { get; private set; }

        public double SampleRate { get; private set; }

        public ulong Start { get; private set; }
        
        public ulong Block { get; private set; }

        public ulong SegmentLength { get; private set; }
    }
}
