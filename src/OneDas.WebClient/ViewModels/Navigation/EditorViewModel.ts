class EditorViewModel extends WorkspaceBase
{
    public CampaignDescriptionSet: KnockoutObservableArray<OneDasCampaignDescriptionViewModel>
    public Project: KnockoutObservable<OneDasProjectViewModel>
    public PrimaryGroupName: KnockoutObservable<string>
    public SecondaryGroupName: KnockoutObservable<string>
    public CampaignName: KnockoutObservable<string>

    public PrimaryGroupNameHasError: KnockoutObservable<boolean>
    public SecondaryGroupNameHasError: KnockoutObservable<boolean>
    public CampaignNameHasError: KnockoutObservable<boolean>

    public PrimaryGroupNameErrorDescription: KnockoutObservable<string>
    public SecondaryGroupNameErrorDescription: KnockoutObservable<string>
    public CampaignNameErrorDescription: KnockoutObservable<string>

    public NewProjectCheckError: KnockoutObservable<boolean>

    constructor(activeProject: KnockoutObservable<OneDasProjectViewModel>)
    {
        super('editor', 'Editor', 'editor.html', activeProject)

        this.CampaignDescriptionSet = ko.observableArray<OneDasCampaignDescriptionViewModel>()
        this.Project = ko.observable<OneDasProjectViewModel>()
        this.PrimaryGroupName = ko.observable<string>("")
        this.SecondaryGroupName = ko.observable<string>("")
        this.CampaignName = ko.observable<string>("")

        this.PrimaryGroupNameHasError = ko.observable<boolean>(false)
        this.SecondaryGroupNameHasError = ko.observable<boolean>(false)
        this.CampaignNameHasError = ko.observable<boolean>(false)

        this.PrimaryGroupNameErrorDescription = ko.observable<string>("")
        this.SecondaryGroupNameErrorDescription = ko.observable<string>("")
        this.CampaignNameErrorDescription = ko.observable<string>("")

        this.NewProjectCheckError = ko.observable(false)
              
        // validation
        this.PrimaryGroupName.subscribe((value) =>
        {
            var result

            if (!this.NewProjectCheckError())
            {
                result = { HasError: false, ErrorDescription: "" }
            }
            else
            {
                result = CheckNamingConvention(value)
            }
            
            this.PrimaryGroupNameHasError(result.HasError)
            this.PrimaryGroupNameErrorDescription(result.ErrorDescription)
            this.NewProjectCheckError(true)
        })

        this.SecondaryGroupName.subscribe((value) =>
        {
            var result

            if (!this.NewProjectCheckError())
            {
                result = { HasError: false, ErrorDescription: "" }
            }
            else
            {
                result = CheckNamingConvention(value)
            }

            this.SecondaryGroupNameHasError(result.HasError)
            this.SecondaryGroupNameErrorDescription(result.ErrorDescription)
            this.NewProjectCheckError(true)
        })

        this.CampaignName.subscribe((value) =>
        {
            var result

            if (!this.NewProjectCheckError())
            {
                result = { HasError: false, ErrorDescription: "" }
            }
            else
            {
                result = CheckNamingConvention(value)
            }

            this.CampaignNameHasError(result.HasError)
            this.CampaignNameErrorDescription(result.ErrorDescription)
            this.NewProjectCheckError(true)
        })
    }

    // commands
    public GetCampaignDescriptions = async () =>
    {
        let campaignDescriptionSet: any[]

        try
        {
            campaignDescriptionSet = await ConnectionManager.InvokeWebClientHub('GetCampaignDescriptions')
            this.CampaignDescriptionSet(campaignDescriptionSet.map(campaignDescription => new OneDasCampaignDescriptionViewModel(campaignDescription)))
        }
        catch (e)
        {
            alert(e.message)
        }
    }

    public CreateNewProject = async () => 
    {
        let projectModel: any

        try
        {
            projectModel = await ConnectionManager.InvokeWebClientHub("CreateProject", this.PrimaryGroupName(), this.SecondaryGroupName(), this.CampaignName())

            this.Project(new OneDasProjectViewModel(projectModel))

            await this.Project().InitializeAsync(projectModel.DataGatewaySettingsSet, projectModel.DataWriterSettingsSet)

            this.NewProjectCheckError(false)
            this.PrimaryGroupName("")

            this.NewProjectCheckError(false)
            this.SecondaryGroupName("")

            this.NewProjectCheckError(false)
            this.CampaignName("")
        }
        catch (e)
        {
            alert(e.message)
        }
    }

    public SaveProject = async () =>
    {
        try
        {
            this.Project().Description.Version(this.Project().Description.Version() + 1)

            await ConnectionManager.InvokeWebClientHub("SaveProject", this.Project().ToModel())
            console.log("OneDAS: project saved")
        }
        catch (e)
        {
            this.Project().Description.Version(this.Project().Description.Version() - 1)
            alert(e.message)
        }
    }

    public OpenProject = async (campaignDescriptionViewModel: OneDasCampaignDescriptionViewModel) =>
    {
        try
        {
            let projectModel: any

            projectModel = await ConnectionManager.InvokeWebClientHub("OpenProject", campaignDescriptionViewModel.ToModel())
            
            this.Project(new OneDasProjectViewModel(projectModel))

            await this.Project().InitializeAsync(projectModel.DataGatewaySettingsSet, projectModel.DataWriterSettingsSet)

            console.log("OneDAS: project opened")
        }
        catch (e)
        {
            alert(e.message)
        }
    }

    public CloseProject = () =>
    {
        this.Project(null)
    }
}