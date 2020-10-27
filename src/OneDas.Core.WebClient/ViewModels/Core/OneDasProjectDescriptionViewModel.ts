class OneDasProjectDescriptionViewModel
{
    public Guid: string
    public Version: KnockoutObservable<number>
    public PrimaryGroupName: string
    public SecondaryGroupName: string
    public ProjectName: string

    constructor(projectDescriptionModel: any)
    {
        this.Guid = projectDescriptionModel.Guid
        this.Version = ko.observable<number>(projectDescriptionModel.Version)
        this.PrimaryGroupName = projectDescriptionModel.PrimaryGroupName
        this.SecondaryGroupName = projectDescriptionModel.SecondaryGroupName
        this.ProjectName = projectDescriptionModel.ProjectName
    }

    public ToModel = () =>
    {
        return {
            Guid: <string>this.Guid,
            Version: <number>this.Version(),
            PrimaryGroupName: <string>this.PrimaryGroupName,
            SecondaryGroupName: <string>this.SecondaryGroupName,
            ProjectName: <string>this.ProjectName,
        }
    }
}