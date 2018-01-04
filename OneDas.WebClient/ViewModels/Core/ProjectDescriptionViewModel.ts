class ProjectDescriptionViewModel
{
    public FormatVersion: number
    public Guid: string
    public CampaignVersion: KnockoutObservable<number>
    public CampaignPrimaryGroup: string
    public CampaignSecondaryGroup: string
    public CampaignName: string

    constructor(projectDescriptionModel: any)
    {
        this.FormatVersion = projectDescriptionModel.FormatVersion
        this.Guid = projectDescriptionModel.Guid
        this.CampaignVersion = ko.observable<number>(projectDescriptionModel.CampaignVersion)
        this.CampaignPrimaryGroup = projectDescriptionModel.CampaignPrimaryGroup
        this.CampaignSecondaryGroup = projectDescriptionModel.CampaignSecondaryGroup
        this.CampaignName = projectDescriptionModel.CampaignName
    }

    public ToModel = () =>
    {
        return {
            FormatVersion: <number>this.FormatVersion,
            CampaignVersion: <number>this.CampaignVersion(),
            Guid: <string>this.Guid,
            CampaignPrimaryGroup: <string>this.CampaignPrimaryGroup,
            CampaignSecondaryGroup: <string>this.CampaignSecondaryGroup,
            CampaignName: <string>this.CampaignName,
        }
    }
}