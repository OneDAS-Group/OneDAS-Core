using OneDas.DataStorage;
using OneDas.Extensibility;
using System.Collections.Generic;

namespace OneDas.Core.Engine
{
    public class DataStorageContext
    {
        public DataStorageContext(List<IExtendedDataStorage> dataStorageSet, DataGatewayExtensionLogicBase dataGateway, DataPort dataPort)
        {
            this.DataStorageSet = dataStorageSet;
            this.DataGateway = dataGateway;
            this.DataPort = dataPort;
        }

        public List<IExtendedDataStorage> DataStorageSet { get; private set; }
        public DataGatewayExtensionLogicBase DataGateway { get; private set; }
        public DataPort DataPort { get; private set; }
    }
}
