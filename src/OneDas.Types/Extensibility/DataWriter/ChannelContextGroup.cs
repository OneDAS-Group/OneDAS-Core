using OneDas.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace OneDas.Extensibility
{
    public class ChannelContextGroup
    {
        public ChannelContextGroup(SampleRateContainer sampleRate, IList<ChannelContext> channelContextSet)
        {
            this.SampleRate = sampleRate;
            this.ChannelContextSet = channelContextSet.ToList();
        }

        public SampleRateContainer SampleRate { get; }
        public List<ChannelContext> ChannelContextSet { get; }
    }
}
