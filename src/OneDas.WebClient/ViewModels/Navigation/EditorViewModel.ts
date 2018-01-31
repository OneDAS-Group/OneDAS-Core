class EditorViewModel extends WorkspaceBase
{
    public ProjectDescriptionSet: KnockoutObservableArray<ProjectDescriptionViewModel>
    public Project: KnockoutObservable<ProjectViewModel>
    public CampaignPrimaryGroup: KnockoutObservable<string>
    public CampaignSecondaryGroup: KnockoutObservable<string>
    public CampaignName: KnockoutObservable<string>

    public CampaignPrimaryGroupHasError: KnockoutObservable<boolean>
    public CampaignSecondaryGroupHasError: KnockoutObservable<boolean>
    public CampaignNameHasError: KnockoutObservable<boolean>

    public CampaignPrimaryGroupErrorDescription: KnockoutObservable<string>
    public CampaignSecondaryGroupErrorDescription: KnockoutObservable<string>
    public CampaignNameErrorDescription: KnockoutObservable<string>

    public NewProjectCheckError: KnockoutObservable<boolean>

    constructor(activeProject: KnockoutObservable<ProjectViewModel>)
    {
        super('editor', 'Editor', 'editor.html', activeProject)

        this.ProjectDescriptionSet = ko.observableArray<ProjectDescriptionViewModel>()
        this.Project = ko.observable<ProjectViewModel>()
        this.CampaignPrimaryGroup = ko.observable<string>("")
        this.CampaignSecondaryGroup = ko.observable<string>("")
        this.CampaignName = ko.observable<string>("")

        this.CampaignPrimaryGroupHasError = ko.observable<boolean>(false)
        this.CampaignSecondaryGroupHasError = ko.observable<boolean>(false)
        this.CampaignNameHasError = ko.observable<boolean>(false)

        this.CampaignPrimaryGroupErrorDescription = ko.observable<string>("")
        this.CampaignSecondaryGroupErrorDescription = ko.observable<string>("")
        this.CampaignNameErrorDescription = ko.observable<string>("")

        this.NewProjectCheckError = ko.observable(false)
              
        // validation
        this.CampaignPrimaryGroup.subscribe((value) =>
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
            
            this.CampaignPrimaryGroupHasError(result.HasError)
            this.CampaignPrimaryGroupErrorDescription(result.ErrorDescription)
            this.NewProjectCheckError(true)
        })

        this.CampaignSecondaryGroup.subscribe((value) =>
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

            this.CampaignSecondaryGroupHasError(result.HasError)
            this.CampaignSecondaryGroupErrorDescription(result.ErrorDescription)
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
    public GetProjectDescriptions = async () =>
    {
        let projectDescriptionSet: any[]

        try
        {
            projectDescriptionSet = await ConnectionManager.InvokeWebClientHub('GetProjectDescriptions')
            this.ProjectDescriptionSet(projectDescriptionSet.map(projectDescription => new ProjectDescriptionViewModel(projectDescription)))
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
            projectModel = await ConnectionManager.InvokeWebClientHub("CreateProject", this.CampaignPrimaryGroup(), this.CampaignSecondaryGroup(), this.CampaignName())

            this.Project(new ProjectViewModel(projectModel))

            await this.Project().InitializeAsync(projectModel.DataGatewaySettingsSet, projectModel.DataWriterSettingsSet)

            this.NewProjectCheckError(false)
            this.CampaignPrimaryGroup("")

            this.NewProjectCheckError(false)
            this.CampaignSecondaryGroup("")

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
            this.Project().Description.CampaignVersion(this.Project().Description.CampaignVersion() + 1)

            await ConnectionManager.InvokeWebClientHub("SaveProject", this.Project().ToModel())
            console.log("OneDAS: project saved")
        }
        catch (e)
        {
            this.Project().Description.CampaignVersion(this.Project().Description.CampaignVersion() - 1)
            alert(e.message)
        }
    }

    public OpenProject = async (projectDescriptionViewModel: ProjectDescriptionViewModel) =>
    {
        try
        {
            let projectModel: any

            projectModel = await ConnectionManager.InvokeWebClientHub("OpenProject", projectDescriptionViewModel.ToModel())
            
            this.Project(new ProjectViewModel(projectModel))

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