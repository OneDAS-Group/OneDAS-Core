using OneDas.DataManagement.Database;
using OneDas.DataManagement.Extensibility;
using System;

namespace OneDas.DataManagement.BlazorExplorer.Core
{
    public class ZipSettings
    {
        public ZipSettings(CampaignInfo campaign,
                           DataReaderExtensionBase nativeDataReader,
                           DataReaderExtensionBase aggregationDataReader,
                           DateTime begin,
                           DateTime end,
                           ulong samplesPerDay,
                           ulong segmentLength)
        {
            this.Campaign = campaign;
            this.NativeDataReader = nativeDataReader;
            this.AggregationDataReader = aggregationDataReader;
            this.Begin = begin;
            this.End = end;
            this.SamplesPerDay = samplesPerDay;
            this.SegmentLength = segmentLength;
        }


        public CampaignInfo Campaign { get; private set; }

        public DataReaderExtensionBase NativeDataReader { get; private set; }

        public DataReaderExtensionBase AggregationDataReader { get; private set; }

        public DateTime Begin { get; private set; }

        public DateTime End { get; private set; }

        public ulong SamplesPerDay { get; private set; }

        public ulong SegmentLength { get; private set; }
    }
}
