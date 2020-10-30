using System.Collections.Generic;
using System.Linq;

namespace OneDas.DataManagement.Database
{
    public class ProjectMetaInfo
    {
        #region Constructors

        public ProjectMetaInfo(string id)
        {
            this.Id = id;
            this.Contact = string.Empty;
            this.ShortDescription = string.Empty;
            this.LongDescription = string.Empty;
            this.Channels = new List<ChannelMetaInfo>();
        }

        private ProjectMetaInfo()
        {
            //
        }

        #endregion

        #region Properties

        public string Id { get; set; }

        public string Contact { get; set; }

        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        public List<ChannelMetaInfo> Channels { get; set; }

        #endregion

        #region Methods

        public void Initialize(ProjectInfo project)
        {
            if (string.IsNullOrWhiteSpace(this.ShortDescription))
                this.ShortDescription = "<no description available>";

            if (string.IsNullOrWhiteSpace(this.LongDescription))
                this.LongDescription = "<no description available>";

            // create missing channel meta instances
            var channelsToAdd = new List<ChannelMetaInfo>();

            foreach (var referenceChannel in project.Channels)
            {
                var exists = this.Channels.Any(channel => channel.Id == referenceChannel.Id);

                if (!exists)
                    channelsToAdd.Add(new ChannelMetaInfo(referenceChannel.Id));
            }

            this.Channels.AddRange(channelsToAdd);
            this.Channels = this.Channels.OrderBy(channel => channel.Id).ToList();
        }

        #endregion
    }
}
