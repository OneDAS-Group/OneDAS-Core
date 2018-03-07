using OneDas.Infrastructure;
using OneDas.Plugin;
using System.Collections.Generic;

namespace OneDas.Engine.Core
{
    public class DataStorageContext
    {
        public DataStorageContext(ChannelHub channelHub, List<IExtendedDataStorage> dataStorageSet, DataGatewayPluginLogicBase dataGateway, DataPort dataPort)
        {
            this.ChannelHub = channelHub;
            this.DataStorageSet = dataStorageSet;
            this.DataGateway = dataGateway;
            this.DataPort = dataPort;
        }

        public ChannelHub ChannelHub { get; private set; }
        public List<IExtendedDataStorage> DataStorageSet { get; private set; }
        public DataGatewayPluginLogicBase DataGateway { get; private set; }
        public DataPort DataPort { get; private set; }
    }
}
