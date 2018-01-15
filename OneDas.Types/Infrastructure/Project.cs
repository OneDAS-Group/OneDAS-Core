using OneDas.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace OneDas.Infrastructure
{
    [DataContract]
    public class Project
    {
        #region "Constructors"

        static Project()
        {
            Project.FormatVersion = 1; // first version
            Project.FormatVersion = 2; // 11.08.2017 (unit_set, transfer_function_set)
        }

        public Project(string campaignPrimaryGroup, string campaignSecondaryGroup, string campaignName): this(campaignPrimaryGroup, campaignSecondaryGroup, campaignName, new List<DataGatewayPluginSettingsBase>(), new List<DataWriterPluginSettingsBase>())
        {
            //
        }  

        public Project(string campaignPrimaryGroup, string campaignSecondaryGroup, string campaignName, IEnumerable<DataGatewayPluginSettingsBase> dataGatewaySettingsSet, IEnumerable<DataWriterPluginSettingsBase> dataWriterSettingsSet)
        {
            int nextId;
            
            this.Description = new ProjectDescription(Project.FormatVersion, 0, Guid.NewGuid(), campaignPrimaryGroup, campaignSecondaryGroup, campaignName);
            this.ChannelHubSet = new List<ChannelHub>();

            this.DataGatewaySettingsSet = dataGatewaySettingsSet;
            this.DataWriterSettingsSet = dataWriterSettingsSet;
            
            nextId = 1;
            this.DataGatewaySettingsSet.ToList().ForEach(x => x.Description.InstanceId = nextId++);

            nextId = 1;
            this.DataWriterSettingsSet.ToList().ForEach(x => x.Description.InstanceId = nextId++);
        }

        #endregion

        #region "Properties"

        public static int FormatVersion { get; private set; }

        [DataMember]
        public ProjectDescription Description { get; set; }

        [DataMember]
        public IList<ChannelHub> ChannelHubSet { get; private set; }

        [DataMember]
        public IEnumerable<DataGatewayPluginSettingsBase> DataGatewaySettingsSet { get; private set; }

        [DataMember]
        public IEnumerable<DataWriterPluginSettingsBase> DataWriterSettingsSet { get; private set; }

        public IList<ChannelHub> ActiveChannelHubSet { get; private set; }

        #endregion

        #region "Methods"

        public void Validate()
        {
            IEnumerable<Guid> guidSet;
            List<ChannelHub> channelHubSet;

            string errorDescription;

            // -> naming convention
            if (!InfrastructureHelper.CheckNamingConvention(this.Description.CampaignPrimaryGroup, out errorDescription))
            {
                throw new ValidationException(ErrorMessage.Project_CampaignPrimaryGroupInvalid);
            }

            if (!InfrastructureHelper.CheckNamingConvention(this.Description.CampaignSecondaryGroup, out errorDescription))
            {
                throw new ValidationException(ErrorMessage.Project_CampaignSecondaryGroupInvalid);
            }

            if (!InfrastructureHelper.CheckNamingConvention(this.Description.CampaignName, out errorDescription))
            {
                throw new ValidationException(ErrorMessage.Project_CampaignNameInvalid);
            }

            if (!this.ChannelHubSet.ToList().TrueForAll(x => InfrastructureHelper.CheckNamingConvention(x.Name, out errorDescription)))
            {
                throw new ValidationException(ErrorMessage.Project_ChannelHubNameInvalid);
            }

            // -> ChannelHub
            guidSet = this.ChannelHubSet.Select(x => x.Guid).ToList();
            channelHubSet = this.ChannelHubSet.Where(channelHub => channelHub.AssociatedDataInput != null).ToList();

            if (guidSet.Count() > guidSet.Distinct().Count())
            {
                throw new ValidationException(ErrorMessage.Project_ChannelHubNotUnqiue);
            }

            // -> data type matching
            if (!channelHubSet.TrueForAll(x => InfrastructureHelper.GetBitLength(x.OneDasDataType, true) == InfrastructureHelper.GetBitLength(x.AssociatedDataInput.OneDasDataType, true)))
            {
                throw new ValidationException(ErrorMessage.Project_DataTypeMismatch);
            }

            // -> data gateway settings
            if (this.DataGatewaySettingsSet.Select(x => x.Description.InstanceId).Count() > this.DataGatewaySettingsSet.Select(x => x.Description.InstanceId).Distinct().Count())
            {
                throw new ValidationException(ErrorMessage.Project_DataGatewaySettingsIdNotUnique);
            }

            this.DataGatewaySettingsSet.ToList().ForEach(dataGatewaySettings => dataGatewaySettings.Validate());

            // -> data writer settings
            if (this.DataWriterSettingsSet.Select(x => x.Description.InstanceId).Count() > this.DataWriterSettingsSet.Select(x => x.Description.InstanceId).Distinct().Count())
            {
                throw new ValidationException(ErrorMessage.Project_DataWriterSettingsIdNotUnique);
            }

            this.DataWriterSettingsSet.ToList().ForEach(dataWriterSettings => dataWriterSettings.Validate());
        }

        private void Initialize()
        {
            IEnumerable<DataPort> dataPortSet;
            IDictionary<DataPort, string> dataPortIdMap;

            // assign correct data-gateway instances to all data ports
            this.DataGatewaySettingsSet.ToList().ForEach(dataGateway =>
            {
                dataGateway.GetDataPortSet().ToList().ForEach(dataPort =>
                {
                    dataPort.AssociatedDataGateway = dataGateway;
                });
            });

            // UpdateMapping()
            dataPortSet = this.DataGatewaySettingsSet.SelectMany(x => x.GetDataPortSet()).ToList();
            dataPortIdMap = dataPortSet.ToDictionary(x => x, x => x.ToUniqueIdentifier());

            this.ChannelHubSet.ToList().ForEach(channelHub =>
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

            this.ActiveChannelHubSet = this.ChannelHubSet.Where(channelHub => channelHub.AssociatedDataInput != null).ToList();
        }

        private bool IsAssociationAllowed(DataPort dataPort, ChannelHub channelHub)
        {
            return InfrastructureHelper.GetBitLength(dataPort.OneDasDataType, true) == InfrastructureHelper.GetBitLength(channelHub.OneDasDataType, true);
        }

        #endregion

        #region "Serialization"

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
            this.Initialize();
        }

        #endregion
    }
}