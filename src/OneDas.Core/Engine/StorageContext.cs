using OneDas.Buffers;
using OneDas.Extensibility;
using System.Collections.Generic;

namespace OneDas.Core.Engine
{
    public class StorageContext
    {
        public StorageContext(List<IExtendedBuffer> buffers, DataGatewayExtensionLogicBase dataGateway, DataPort dataPort)
        {
            this.Buffers = buffers;
            this.DataGateway = dataGateway;
            this.DataPort = dataPort;
        }

        public List<IExtendedBuffer> Buffers { get; private set; }
        public DataGatewayExtensionLogicBase DataGateway { get; private set; }
        public DataPort DataPort { get; private set; }
    }
}
