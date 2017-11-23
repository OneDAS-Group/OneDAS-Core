using OneDas.Infrastructure;
using OneDas.Plugin;

namespace OneDas.Main.Core
{
    public class DataStorageContext
    {
        public DataStorageContext(ChannelHub channelHub, DataGatewayPluginLogicBase dataGateway, DataPortBase dataPort)
        {
            this.ChannelHub = channelHub;
            this.DataGateway = dataGateway;
            this.DataPort = dataPort;
        }

        public ChannelHub ChannelHub { get; private set; }
        public DataGatewayPluginLogicBase DataGateway { get; private set; }
        public DataPortBase DataPort { get; private set; }
    }
}
