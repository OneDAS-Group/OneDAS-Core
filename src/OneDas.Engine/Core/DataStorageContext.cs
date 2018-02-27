using OneDas.Plugin;

namespace OneDas.Engine.Core
{
    public class DataStorageContext
    {
        public DataStorageContext(ChannelHub channelHub, DataGatewayPluginLogicBase dataGateway, DataPort dataPort)
        {
            this.ChannelHub = channelHub;
            this.DataGateway = dataGateway;
            this.DataPort = dataPort;
        }

        public ChannelHub ChannelHub { get; private set; }
        public DataGatewayPluginLogicBase DataGateway { get; private set; }
        public DataPort DataPort { get; private set; }
    }
}
