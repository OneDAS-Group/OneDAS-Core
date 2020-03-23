using OneDas.DataManagement.Infrastructure;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;

namespace OneDas.DataManagement.Connector
{
    public class ExportSettings
    {
        #region Constructors

        public ExportSettings(DateTime dateTimeBegin, DateTime dateTimeEnd, FileFormat fileFormat, FileGranularity fileGranularity, List<string> channelNames)
        {
            this.DateTimeBegin = dateTimeBegin;
            this.DateTimeEnd = dateTimeEnd;
            this.FileFormat = fileFormat;
            this.FileGranularity = fileGranularity;
            this.ChannelNames = channelNames;
        }

        #endregion

        #region Properties

        public DateTime DateTimeBegin { get; private set; }

        public DateTime DateTimeEnd { get; private set; }

        public FileFormat FileFormat { get; private set; }

        public FileGranularity FileGranularity { get; private set; }

        public List<string> ChannelNames { get; private set; }

        #endregion
    }
}
