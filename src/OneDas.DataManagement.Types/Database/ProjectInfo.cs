using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OneDas.DataManagement.Database
{
    [DebuggerDisplay("{Id,nq}")]
    public class ProjectInfo : ProjectElement
    {
        #region "Constructors"

        public ProjectInfo(string id) : base(id, null)
        {
            this.Channels = new List<ChannelInfo>();
        }

        private ProjectInfo()
        {
            //
        }

        #endregion

        #region "Properties"

        public DateTime ProjectStart { get; set; }

        public DateTime ProjectEnd { get; set; }

        public List<ChannelInfo> Channels { get; set; }

        #endregion

        #region "Methods"

        public void Merge(ProjectInfo project, ChannelMergeMode mergeMode)
        {
            if (this.Id != project.Id)
                throw new Exception("The project to be merged has a different ID.");

            // merge channels
            List<ChannelInfo> newChannels = new List<ChannelInfo>();

            foreach (var channel in project.Channels)
            {
                var referenceChannel = this.Channels.FirstOrDefault(current => current.Id == channel.Id);

                if (referenceChannel != null)
                    referenceChannel.Merge(channel, mergeMode);
                else
                    newChannels.Add(channel);

                channel.Parent = this;
            }

            this.Channels.AddRange(newChannels);

            // merge other data
            if (this.ProjectStart == DateTime.MinValue)
                this.ProjectStart = project.ProjectStart;
            else
                this.ProjectStart = new DateTime(Math.Min(this.ProjectStart.Ticks, project.ProjectStart.Ticks));

            if (this.ProjectEnd == DateTime.MinValue)
                this.ProjectEnd = project.ProjectEnd;
            else
                this.ProjectEnd = new DateTime(Math.Max(this.ProjectEnd.Ticks, project.ProjectEnd.Ticks));
        }

        public override string GetPath()
        {
            return this.Id;
        }

        public override IEnumerable<ProjectElement> GetChilds()
        {
            return this.Channels;
        }

        public override void Initialize()
        {
            base.Initialize();

            foreach (var channel in this.Channels)
            {
                channel.Parent = this;
                channel.Initialize();
            }
        }

        #endregion
    }
}
