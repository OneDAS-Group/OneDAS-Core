using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Server;
using System.Collections.Generic;

namespace OneDas.Extension.OpcUa
{
    public class OpcUaServer : StandardServer
    {
        public OpcUaNodeManager CustomNodeManager { get; private set; }
        private OpcUaServerSettings _settings;
        private ILogger _logger;

        public OpcUaServer(OpcUaServerSettings settings, ILogger logger)
        {
            _settings = settings;
            _logger = logger;
        }

        protected override MasterNodeManager CreateMasterNodeManager(IServerInternal server, ApplicationConfiguration configuration)
        {
            List<INodeManager> nodeManagers = new List<INodeManager>();

            // create the custom node managers.
            this.CustomNodeManager = new OpcUaNodeManager(server, configuration, _settings);
            nodeManagers.Add(this.CustomNodeManager);

            // create master node manager.
            return new MasterNodeManager(server, configuration, null, nodeManagers.ToArray());
        }

        protected override ServerProperties LoadServerProperties()
        {
            ServerProperties properties = new ServerProperties();

            properties.ManufacturerName = "OneDAS Group";
            properties.ProductName = "OneDAS OPC-UA";
            properties.ProductUri = "https://github.com/OneDAS-Group";
            properties.SoftwareVersion = Utils.GetAssemblySoftwareVersion();
            properties.BuildNumber = Utils.GetAssemblyBuildNumber();
            properties.BuildDate = Utils.GetAssemblyTimestamp();

            return properties;
        }

        protected override void OnServerStarted(IServerInternal server)
        {
            _logger.LogInformation("OPC-UA server started.");
            base.OnServerStarted(server);
        }

    }
}
