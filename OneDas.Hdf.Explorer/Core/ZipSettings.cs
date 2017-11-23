using System;
using System.Collections.Generic;

namespace OneDas.Hdf.Explorer.Core
{
    public class ZipSettings
    {
        public ZipSettings(DateTime dateTimeBegin, KeyValuePair<string, Dictionary<string, List<string>>> campaignInfo, long sourceFileId, double sampleRate, ulong start, ulong stride, ulong block, ulong count, ulong segmentLength)
        {
            this.DateTimeBegin = dateTimeBegin;
            this.CampaignInfo = campaignInfo;
            this.SourceFileId = sourceFileId;
            this.SampleRate = sampleRate;
            this.Start = start;
            this.Stride = stride;
            this.Block = block;
            this.Count = count;
            this.SegmentLength = segmentLength;
        }

        public DateTime DateTimeBegin { get; private set; }
        public KeyValuePair<string, Dictionary<string, List<string>>> CampaignInfo { get; private set; }
        public long SourceFileId { get; private set; }
        public double SampleRate { get; private set; }
        public ulong Start { get; private set; }
        public ulong Stride { get; private set; }
        public ulong Block { get; private set; }
        public ulong Count { get; private set; }
        public ulong SegmentLength { get; private set; }
    }
}
