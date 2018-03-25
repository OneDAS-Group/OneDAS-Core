using OneDas.Engine.Core;
using System.Collections.Generic;

namespace OneDas.WebServer.Core
{
    public class LiveViewSubscription
    {
        public LiveViewSubscription(int id, IList<ChannelHubBase> channelHubSet)
        {
            this.Id = id;
            this.ChannelHubSet = channelHubSet;
        }

        public int Id { get; }

        public IList<ChannelHubBase> ChannelHubSet { get; }
    }
}