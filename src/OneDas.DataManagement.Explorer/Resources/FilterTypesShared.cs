// WARNING: DO NOT EDIT OR MOVE THIS FILE UNLESS YOU KNOW WHAT YOU ARE DOING!

using System;
using System.Collections.Generic;

namespace OneDas.DataManagement.Explorer.Filters
{
    public static class FilterConstants
    {
        public static string SharedProjectID { get; } = "/IN_MEMORY/FILTERS/SHARED";
    }

    public record FilterChannel
    {
        #region Constructors

        public FilterChannel()
        {
            //
        }

        public FilterChannel(string projectId, string channelName, string group, string unit)
        {
            this.ProjectId = projectId;
            this.ChannelName = channelName;
            this.Group = group;
            this.Unit = unit;
        }

        #endregion

        #region Properties

        public string ProjectId { get; init; } = FilterConstants.SharedProjectID;
        public string ChannelName { get; init; } = string.Empty;
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

        public abstract void Filter(DateTime begin, DateTime end, FilterChannel filterChannel, Func<string, string, string, DateTime, DateTime, double[]> getData, double[] result);

        protected abstract List<FilterChannel> GetFilters();

        #endregion
    }
}