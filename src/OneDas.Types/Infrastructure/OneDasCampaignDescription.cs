using System;
using System.Runtime.Serialization;

namespace OneDas.Infrastructure
{
    [DataContract]
    public class OneDasCampaignDescription
    {
        public OneDasCampaignDescription(Guid guid, int version, string primaryGroupName, string secondaryGroupName, string campaignName)
        {
            this.Guid = guid;
            this.Version = version;
            this.PrimaryGroupName = primaryGroupName;
            this.SecondaryGroupName = secondaryGroupName;
            this.CampaignName = campaignName;
        }

        [DataMember]
        public Guid Guid { get; set; }

        [DataMember]
        public int Version { get; set; }

        [DataMember]
        public string PrimaryGroupName { get; set; }

        [DataMember]
        public string SecondaryGroupName { get; set; }

        [DataMember]
        public string CampaignName { get; set; }

        public void Validate()
        {
            string errorMessage;

            if (this.Version < 0)
            {
                throw new Exception(ErrorMessage.OneDasCampaignDescription_InvalidVersion);
            }

            if (!OneDasUtilities.CheckNamingConvention(this.PrimaryGroupName, out errorMessage))
            {
                throw new Exception($"The PrimaryGroupName is invalid: { errorMessage }");
            }

            if (!OneDasUtilities.CheckNamingConvention(this.SecondaryGroupName, out errorMessage))
            {
                throw new Exception($"The SecondaryGroupName is invalid: { errorMessage }");
            }

            if (!OneDasUtilities.CheckNamingConvention(this.CampaignName, out errorMessage))
            {
                throw new Exception($"The CampaignName is invalid: { errorMessage }");
            }
        }
    }
}
