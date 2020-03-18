using OneDas.Infrastructure;
using System;
using System.Collections.Generic;

namespace OneDas.DataManagement.Interface
{
    public class DownloadSettings
    {
        #region Constructors

        public DownloadSettings(DateTime dateTimeBegin, DateTime dateTimeEnd, FileGranularity fileGranularity, List<string> channelNames)
        {
            this.DateTimeBegin = dateTimeBegin;
            this.DateTimeEnd = dateTimeEnd;
            this.FileGranularity = fileGranularity;
            this.ChannelNames = channelNames;
        }

        #endregion

        #region Properties

        public DateTime DateTimeBegin { get; private set; }

        public DateTime DateTimeEnd { get; private set; }

        public FileGranularity FileGranularity { get; private set; }

        public List<string> ChannelNames { get; private set; }

        #endregion
    }
}
