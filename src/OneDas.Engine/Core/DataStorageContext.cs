using OneDas.Infrastructure;
using OneDas.Plugin;
using System.Collections.Generic;

namespace OneDas.Engine.Core
{
    public class DataStorageContext
    {
        public DataStorageContext(List<IExtendedDataStorage> dataStorageSet, DataGatewayPluginLogicBase dataGateway, DataPort dataPort)
        {
            this.DataStorageSet = dataStorageSet;
            this.DataGateway = dataGateway;
            this.DataPort = dataPort;
        }

        public List<IExtendedDataStorage> DataStorageSet { get; private set; }
        public DataGatewayPluginLogicBase DataGateway { get; private set; }
        public DataPort DataPort { get; private set; }
    }
}
