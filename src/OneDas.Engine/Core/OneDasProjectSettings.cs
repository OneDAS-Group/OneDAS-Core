using OneDas.Infrastructure;
using OneDas.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace OneDas.Engine.Core
{
    [DataContract]
    public class OneDasProjectSettings
    {
        #region "Constructors"

        static OneDasProjectSettings()
        {
            OneDasProjectSettings.CurrentFormatVersion = 1; // first version
            OneDasProjectSettings.CurrentFormatVersion = 2; // 11.08.2017 (unit_set, transfer_function_set)
        }

        public OneDasProjectSettings(string primaryGroupName, string secondaryGroupName, string campaignName): this(primaryGroupName, secondaryGroupName, campaignName, new List<DataGatewayPluginSettingsBase>(), new List<DataWriterPluginSettingsBase>())
        {
            //
        }  

        public OneDasProjectSettings(string primaryGroupName, string campaignSecondaryGroup, string campaignName, IEnumerable<DataGatewayPluginSettingsBase> dataGatewaySettingsSet, IEnumerable<DataWriterPluginSettingsBase> dataWriterSettingsSet)
        {
            int nextId;
            
            this.Description = new OneDasCampaignDescription(Guid.NewGuid(), 0, primaryGroupName, campaignSecondaryGroup, campaignName);
            this.ChannelHubSet = new List<ChannelHub>();

            this.DataGatewaySettingsSet = dataGatewaySettingsSet;
            this.DataWriterSettingsSet = dataWriterSettingsSet;
            
            nextId = 1;
            this.DataGatewaySettingsSet.ToList().ForEach(settings => settings.Description.InstanceId = nextId++);

            nextId = 1;
            this.DataWriterSettingsSet.ToList().ForEach(settings => settings.Description.InstanceId = nextId++);

            this.FormatVersion = OneDasProjectSettings.CurrentFormatVersion;
        }

        #endregion

        #region "Properties"

        public static int CurrentFormatVersion { get; private set; }

        [DataMember]
        public int FormatVersion { get; set; }

        [DataMember]
        public OneDasCampaignDescription Description { get; set; }

        [DataMember]
        public List<ChannelHub> ChannelHubSet { get; private set; }

        [DataMember]
        public IEnumerable<DataGatewayPluginSettingsBase> DataGatewaySettingsSet { get; private set; }

        [DataMember]
        public IEnumerable<DataWriterPluginSettingsBase> DataWriterSettingsSet { get; private set; }

        #endregion

        #region "Methods"

        public OneDasProjectSettings Clone()
        {
            return (OneDasProjectSettings)this.MemberwiseClone();
        }

        public void Validate()
        {
            IEnumerable<Guid> guidSet;
            List<ChannelHub> channelHubSet;

            string errorDescription;

            // -> naming convention
            if (!OneDasUtilities.CheckNamingConvention(this.Description.PrimaryGroupName, out errorDescription))
            {
                throw new ValidationException(ErrorMessage.OneDasProject_PrimaryGroupNameInvalid);
            }

            if (!OneDasUtilities.CheckNamingConvention(this.Description.SecondaryGroupName, out errorDescription))
            {
                throw new ValidationException(ErrorMessage.OneDasProject_SecondaryGroupNameInvalid);
            }

            if (!OneDasUtilities.CheckNamingConvention(this.Description.CampaignName, out errorDescription))
            {
                throw new ValidationException(ErrorMessage.OneDasProject_CampaignNameInvalid);
            }

            if (!this.ChannelHubSet.ToList().TrueForAll(x => OneDasUtilities.CheckNamingConvention(x.Name, out errorDescription)))
            {
                throw new ValidationException(ErrorMessage.OneDasProject_ChannelHubNameInvalid);
            }

            // -> ChannelHub
            guidSet = this.ChannelHubSet.Select(x => x.Guid).ToList();
            channelHubSet = this.ChannelHubSet.Where(channelHub => channelHub.AssociatedDataInput != null).ToList();

            if (guidSet.Count() > guidSet.Distinct().Count())
            {
                throw new ValidationException(ErrorMessage.OneDasProject_ChannelHubNotUnqiue);
            }

            // -> data type matching
            if (!channelHubSet.TrueForAll(x => OneDasUtilities.GetBitLength(x.DataType, true) == OneDasUtilities.GetBitLength(x.AssociatedDataInput.DataType, true)))
            {
                throw new ValidationException(ErrorMessage.OneDasProject_DataTypeMismatch);
            }

            // -> data gateway settings
            if (this.DataGatewaySettingsSet.Select(x => x.Description.InstanceId).Count() > this.DataGatewaySettingsSet.Select(x => x.Description.InstanceId).Distinct().Count())
            {
                throw new ValidationException(ErrorMessage.OneDasProject_DataGatewaySettingsIdNotUnique);
            }

            this.DataGatewaySettingsSet.ToList().ForEach(dataGatewaySettings => dataGatewaySettings.Validate());

            // -> data writer settings
            if (this.DataWriterSettingsSet.Select(x => x.Description.InstanceId).Count() > this.DataWriterSettingsSet.Select(x => x.Description.InstanceId).Distinct().Count())
            {
                throw new ValidationException(ErrorMessage.OneDasProject_DataWriterSettingsIdNotUnique);
            }

            this.DataWriterSettingsSet.ToList().ForEach(dataWriterSettings => dataWriterSettings.Validate());
        }

        #endregion
    }
}