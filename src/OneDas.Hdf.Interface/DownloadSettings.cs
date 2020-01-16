using System;
using System.Collections.Generic;

namespace OneDas.Hdf.Interface
{
    public class DownloadSettings
    {
        public DateTime DateTimeBegin { get; private set; }
        public DateTime DateTimeEnd { get; private set; }
        public FileGranularity FileGranularity { get; private set; }
        public string SampleRateDescription { get; private set; }
        public string CampaignPath { get; private set; }
        public List<string> VariableNames { get; private set; }

        public DownloadSettings(DateTime dateTimeBegin, DateTime dateTimeEnd, FileGranularity fileGranularity, string sampleRateDescription, string campaignPath, List<string> variableNames)
        {
            this.DateTimeBegin = dateTimeBegin;
            this.DateTimeEnd = dateTimeEnd;
            this.FileGranularity = fileGranularity;
            this.SampleRateDescription = sampleRateDescription;
            this.CampaignPath = campaignPath;
            this.VariableNames = variableNames;
        }
    }
}
