using Opc.Ua;
using Opc.Ua.Server;
using System;
using System.Collections.Generic;

// http://opcfoundation.github.io/UA-.NETStandard/help/index.htm#server_development.htm

namespace OneDas.Extension.OpcUa
{
    public class OpcUaNodeManager : CustomNodeManager2
    {
        public List<BaseDataVariableState> VariableSet { get; private set; }

        private const string NAMESPACE = "http://iwes.fraunhofer.de/UA/ITS/";
        private OpcUaServerSettings _settings;

        public OpcUaNodeManager(IServerInternal server, ApplicationConfiguration configuration, OpcUaServerSettings settings) :  base(server, configuration, OpcUaNodeManager.NAMESPACE)
        {
            this.SystemContext.NodeIdFactory = this;
            this.VariableSet = new List<BaseDataVariableState>();

            _settings = settings;
        }

        #region INodeManager

        public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            lock (this.Lock)
            {
                int count;
                FolderState root;
                IList<IReference> references;

                count = 0;
                references = null;

                if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out references))
                {
                    externalReferences[ObjectIds.ObjectsFolder] = references = new List<IReference>();
                }

                root = this.CreateFolder(null, "Data", "Data");
                root.AddReference(ReferenceTypes.Organizes, true, ObjectIds.ObjectsFolder);
                references.Add(new NodeStateReference(ReferenceTypes.Organizes, false, root.NodeId));
                root.EventNotifier = EventNotifiers.SubscribeToEvents;

                this.AddRootNotifier(root);

                //
                //FolderState myFolder = CreateFolder(root, "/MyFolder", "MyFolder");
                //this.ChannelSet.Add(CreateChannel(root, "/MyFolder/MyNode1", "Var1", BuiltInType.Float, ValueRanks.Scalar));

                _settings.GetOutputModuleSet().ForEach(module =>
                {
                    string name;

                    this.VariableSet.Add(this.CreateVariable(root, "channel_0", "channel_0", BuiltInType.Int64, ValueRanks.Scalar));

                    for (int i = 0; i < module.Size; i++)
                    {
                        name = $"channel_{count+1}";
                        this.VariableSet.Add(this.CreateVariable(root, $"{name}", name, OpcUaUtilities.GetOpcUaDataTypeFromOneDasDataType(module.DataType), ValueRanks.Scalar));
                        count++;
                    }
                });
               
                //root.AddChild(myFolder);

                this.AddPredefinedNode(this.SystemContext, root);
            }
        }

        private FolderState CreateFolder(NodeState parent, string path, string name)
        {
            FolderState folder = new FolderState(parent);

            folder.SymbolicName = name;
            folder.ReferenceTypeId = ReferenceTypes.Organizes;
            folder.TypeDefinitionId = ObjectTypeIds.FolderType;
            folder.NodeId = new NodeId(path, NamespaceIndex);
            folder.BrowseName = new QualifiedName(path, NamespaceIndex);
            folder.DisplayName = new LocalizedText("en", name);
            folder.WriteMask = AttributeWriteMask.None;
            folder.UserWriteMask = AttributeWriteMask.None;
            folder.EventNotifier = EventNotifiers.None;

            if (parent != null)
            {
                parent.AddChild(folder);
            }

            return folder;
        }

        private BaseDataVariableState CreateVariable(NodeState parent, string path, string name, BuiltInType dataType, int valueRank)
        {
            return this.CreateVariable(parent, path, name, (uint)dataType, valueRank);
        }

        private BaseDataVariableState CreateVariable(NodeState parent, string path, string name, NodeId dataType, int valueRank)
        {
            BaseDataVariableState channel = new BaseDataVariableState(parent);

            channel.SymbolicName = name;
            channel.ReferenceTypeId = ReferenceTypes.Organizes;
            channel.TypeDefinitionId = VariableTypeIds.BaseDataVariableType;
            channel.NodeId = new NodeId(path, NamespaceIndex);
            channel.BrowseName = new QualifiedName(path, NamespaceIndex);
            channel.DisplayName = new LocalizedText("en", name);
            channel.WriteMask = AttributeWriteMask.DisplayName | AttributeWriteMask.Description;
            channel.UserWriteMask = AttributeWriteMask.DisplayName | AttributeWriteMask.Description;
            channel.DataType = dataType;
            channel.ValueRank = valueRank;
            channel.AccessLevel = AccessLevels.CurrentReadOrWrite;
            channel.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
            channel.Historizing = false;
            channel.StatusCode = StatusCodes.Good;
            channel.Timestamp = DateTime.UtcNow;

            // this will causes errors if e.g. the data type is actually int
            channel.Value = (float)0.0;

            if (valueRank == ValueRanks.OneDimension)
            {
                channel.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0 });
            }
            else if (valueRank == ValueRanks.TwoDimensions)
            {
                channel.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0, 0 });
            }

            if (parent != null)
            {
                parent.AddChild(channel);
            }

            return channel;
        }

        protected override NodeHandle GetManagerHandle(ServerSystemContext context, NodeId nodeId, IDictionary<NodeId, NodeState> cache)
        {
            lock (Lock)
            {
                // quickly exclude nodes that are not in the namespace. 
                if (!this.IsNodeIdInNamespace(nodeId))
                {
                    return null;
                }

                NodeState node = null;

                if (!PredefinedNodes.TryGetValue(nodeId, out node))
                {
                    return null;
                }

                NodeHandle handle = new NodeHandle();

                handle.NodeId = nodeId;
                handle.Node = node;
                handle.Validated = true;

                return handle;
            }
        }

        protected override NodeState ValidateNode(ServerSystemContext context, NodeHandle handle, IDictionary<NodeId, NodeState> cache)
        {
            // not valid if no root.
            if (handle == null)
            {
                return null;
            }

            // check if previously validated.
            if (handle.Validated)
            {
                return handle.Node;
            }

            // TBD

            return null;
        }

        #endregion
    }
}
