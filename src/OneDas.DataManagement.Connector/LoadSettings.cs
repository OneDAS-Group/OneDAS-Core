using System;
using System.Collections.Generic;

namespace OneDas.DataManagement.Connector
{
    public class LoadSettings
    {
        #region Constructors

        public LoadSettings(DateTime dateTimeBegin, DateTime dateTimeEnd, List<string> channelNames)
        {
            this.DateTimeBegin = dateTimeBegin;
            this.DateTimeEnd = dateTimeEnd;
            this.ChannelNames = channelNames;
        }

        #endregion

        #region Properties

        public DateTime DateTimeBegin { get; private set; }

        public DateTime DateTimeEnd { get; private set; }

        public List<string> ChannelNames { get; private set; }

        #endregion
    }
}
