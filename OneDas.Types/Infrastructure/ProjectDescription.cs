using System;
using System.Runtime.Serialization;

namespace OneDas.Infrastructure
{
    [DataContract]
    public class ProjectDescription
    {
        public ProjectDescription(int formatVersion, int campaignVersion, Guid guid, string campaignPrimaryGroup, string campaignSecondaryGroup, string campaignName)
        {
            this.FormatVersion = formatVersion;
            this.CampaignVersion = campaignVersion;
            this.Guid = guid;
            this.CampaignPrimaryGroup = campaignPrimaryGroup;
            this.CampaignSecondaryGroup = campaignSecondaryGroup;
            this.CampaignName = campaignName;
        }

        [DataMember]
        public int FormatVersion { get; set; }

        [DataMember]
        public int CampaignVersion { get; set; }

        [DataMember]
        public Guid Guid { get; set; }

        [DataMember]
        public string CampaignPrimaryGroup { get; set; }

        [DataMember]
        public string CampaignSecondaryGroup { get; set; }

        [DataMember]
        public string CampaignName { get; set; }

        public void Validate()
        {
            string errorMessage;

            if (this.FormatVersion < 0)
            {
                throw new ValidationException(ErrorMessage.ProjectDescription_InvalidFormatVersion);
            }

            if (this.CampaignVersion < 0)
            {
                throw new ValidationException(ErrorMessage.ProjectDescription_InvalidCampaignVersion);
            }

            if (!InfrastructureHelper.CheckNamingConvention(this.CampaignPrimaryGroup, out errorMessage))
            {
                throw new ValidationException($"The CampaignPrimaryGroup is invalid: { errorMessage }");
            }

            if (!InfrastructureHelper.CheckNamingConvention(this.CampaignSecondaryGroup, out errorMessage))
            {
                throw new ValidationException($"The CampaignSecondaryGroup is invalid: { errorMessage }");
            }

            if (!InfrastructureHelper.CheckNamingConvention(this.CampaignName, out errorMessage))
            {
                throw new ValidationException($"The CampaignName is invalid: { errorMessage }");
            }
        }
    }
}
