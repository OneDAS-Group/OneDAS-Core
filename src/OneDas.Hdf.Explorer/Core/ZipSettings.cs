using System;
using System.Collections.Generic;

namespace OneDas.Hdf.Explorer.Core
{
    public class ZipSettings
    {
        public ZipSettings(DateTime dateTimeBegin, KeyValuePair<string, Dictionary<string, List<string>>> campaignInfo, IDataLake dataLake, double sampleRate, ulong start, ulong block, ulong segmentLength)
        {
            this.DateTimeBegin = dateTimeBegin;
            this.CampaignInfo = campaignInfo;
            this.DataLake = dataLake;
            this.SampleRate = sampleRate;
            this.Start = start;
            this.Block = block;
            this.SegmentLength = segmentLength;
        }

        public DateTime DateTimeBegin { get; private set; }
        public KeyValuePair<string, Dictionary<string, List<string>>> CampaignInfo { get; private set; }
        public IDataLake DataLake { get; private set; }
        public double SampleRate { get; private set; }
        public ulong Start { get; private set; }
        public ulong Block { get; private set; }
        public ulong SegmentLength { get; private set; }
    }
}
