using OneDas.DataManagement.Extensibility;
using System;
using System.Collections.Generic;

namespace OneDas.Hdf.Explorer.Core
{
    public class ZipSettings
    {
        public ZipSettings(DateTime dateTimeBegin, KeyValuePair<string, Dictionary<string, List<string>>> campaignInfo, IDatabase database, double sampleRate, ulong start, ulong block, ulong segmentLength)
        {
            this.DateTimeBegin = dateTimeBegin;
            this.CampaignInfo = campaignInfo;
            this.Database = database;
            this.SampleRate = sampleRate;
            this.Start = start;
            this.Block = block;
            this.SegmentLength = segmentLength;
        }

        public DateTime DateTimeBegin { get; private set; }
        public KeyValuePair<string, Dictionary<string, List<string>>> CampaignInfo { get; private set; }
        public IDatabase Database { get; private set; }
        public double SampleRate { get; private set; }
        public ulong Start { get; private set; }
        public ulong Block { get; private set; }
        public ulong SegmentLength { get; private set; }
    }
}
