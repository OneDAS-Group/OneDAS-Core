// WARNING: DO NOT EDIT OR MOVE THIS FILE UNLESS YOU KNOW WHAT YOU ARE DOING!

using System;
using System.Collections.Generic;

namespace OneDas.DataManagement.Explorer.Filters
{
    public record FilterChannel
    {
        #region Constructors

        public FilterChannel()
        {
            //
        }

        public FilterChannel(string projectId, string channelId, string group, string unit)
        {
            this.ProjectId = projectId;
            this.ChannelId = channelId; // the actual channel ID derives from this, so it is really a (human-readable) ID here
            this.Group = group;
            this.Unit = unit;
        }

        #endregion

        #region Properties

        public string ProjectId { get; init; } = string.Empty;
        public string ChannelId { get; init; } = string.Empty;
        public string Group { get; init; } = string.Empty;
        public string Unit { get; init; } = string.Empty;

        #endregion
    }

    public abstract class FilterProviderBase
    {
        #region Fields

        private List<FilterChannel> _filters;

        #endregion

        #region Constructors

        public FilterProviderBase()
        {
            _filters = this.GetFilters();
        }

        #endregion

        #region Properties

        public IReadOnlyList<FilterChannel> Filters => _filters;

        #endregion

        #region Methods

        public abstract void Filter(FilterChannel filterChannel, DateTime begin, DateTime end, Func<string, string, string, double[]> getData, double[] result);

        protected abstract List<FilterChannel> GetFilters();

        #endregion
    }
}