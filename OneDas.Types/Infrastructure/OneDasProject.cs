using OneDas.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace OneDas.Infrastructure
{
    public class OneDasProject
    {
        #region "Constructors"

        public OneDasProject(IPluginProvider pluginProvider, OneDasProjectSettings projectSettings)
        {
            this.Settings = projectSettings;

            this.DataGatewaySet = this.Settings.DataGatewaySettingsSet.Select(settings => pluginProvider.BuildLogic<DataGatewayPluginLogicBase>(settings)).ToList();
            this.DataWriterSet = this.Settings.DataWriterSettingsSet.Select(settings => pluginProvider.BuildLogic<DataWriterPluginLogicBase>(settings)).ToList();

            this.UpdateMapping();
        }

        #endregion

        #region "Properties"

        public List<DataGatewayPluginLogicBase> DataGatewaySet { get; private set; }

        public List<DataWriterPluginLogicBase> DataWriterSet { get; private set; }

        public List<ChannelHub> ActiveChannelHubSet { get; private set; }

        public OneDasProjectSettings Settings { get; private set; }

        #endregion

        #region "Methods"

        private void UpdateMapping()
        {
            IEnumerable<DataPort> dataPortSet;
            IDictionary<DataPort, string> dataPortIdMap;

            // assign correct data-gateway instances to all data ports
            this.DataGatewaySet.ForEach(dataGateway =>
            {
                dataGateway.GetDataPortSet().ToList().ForEach(dataPort =>
                {
                    dataPort.AssociatedDataGateway = dataGateway;
                });
            });

            // get data ports
            dataPortSet = this.DataGatewaySet.SelectMany(x => x.GetDataPortSet()).ToList();

            // generate unique identifiers for each data port
            dataPortIdMap = dataPortSet.ToDictionary(x => x, x =>
            {
                return $"{ x.AssociatedDataGateway.Settings.Description.Id } ({ x.AssociatedDataGateway.Settings.Description.InstanceId }) / { x.GetId() }";
            });

            // update mapping
            this.Settings.ChannelHubSet.ToList().ForEach(channelHub =>
            {
                string inputId = channelHub.AssociatedDataInputId;

                if (!string.IsNullOrWhiteSpace(inputId))
                {
                    DataPort foundDataPort = dataPortSet.FirstOrDefault(dataPort => dataPortIdMap[dataPort] == inputId);

                    if (foundDataPort != null && this.IsAssociationAllowed(foundDataPort, channelHub))
                    {
                        channelHub.SetAssociation(foundDataPort);
                    }
                }

                foreach (string outputId in channelHub.AssociatedDataOutputIdSet)
                {
                    DataPort foundDataPort = dataPortSet.FirstOrDefault(dataPort => dataPortIdMap[dataPort] == outputId);

                    if (foundDataPort != null && this.IsAssociationAllowed(foundDataPort, channelHub))
                    {
                        channelHub.SetAssociation(foundDataPort);
                    }
                }
            });

            this.ActiveChannelHubSet = this.Settings.ChannelHubSet.Where(channelHub => channelHub.AssociatedDataInput != null).ToList();
        }

        private bool IsAssociationAllowed(DataPort dataPort, ChannelHub channelHub)
        {
            return InfrastructureHelper.GetBitLength(dataPort.OneDasDataType, true) == InfrastructureHelper.GetBitLength(channelHub.OneDasDataType, true);
        }

        #endregion

        #region "IDisposable Support"

        private bool isDisposed;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    this.DataGatewaySet.ForEach(dataGateway => dataGateway.Dispose());
                    this.DataWriterSet.ForEach(dataWriter => dataWriter.Dispose());
                }
            }

            isDisposed = true;
        }

        #endregion
    }
}