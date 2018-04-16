class OneDasCampaignDescriptionViewModel
{
    public Guid: string
    public Version: KnockoutObservable<number>
    public PrimaryGroupName: string
    public SecondaryGroupName: string
    public CampaignName: string

    constructor(campaignDescriptionModel: any)
    {
        this.Guid = campaignDescriptionModel.Guid
        this.Version = ko.observable<number>(campaignDescriptionModel.Version)
        this.PrimaryGroupName = campaignDescriptionModel.PrimaryGroupName
        this.SecondaryGroupName = campaignDescriptionModel.SecondaryGroupName
        this.CampaignName = campaignDescriptionModel.CampaignName
    }

    public ToModel = () =>
    {
        return {
            Guid: <string>this.Guid,
            Version: <number>this.Version(),
            PrimaryGroupName: <string>this.PrimaryGroupName,
            SecondaryGroupName: <string>this.SecondaryGroupName,
            CampaignName: <string>this.CampaignName,
        }
    }
}