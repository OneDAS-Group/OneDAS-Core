class EditorViewModel extends WorkspaceBase
{
    public ProjectDescriptionSet: KnockoutObservableArray<OneDasProjectDescriptionViewModel>
    public Project: KnockoutObservable<OneDasProjectViewModel>
    public PrimaryGroupName: KnockoutObservable<string>
    public SecondaryGroupName: KnockoutObservable<string>
    public ProjectName: KnockoutObservable<string>

    public PrimaryGroupNameHasError: KnockoutObservable<boolean>
    public SecondaryGroupNameHasError: KnockoutObservable<boolean>
    public ProjectNameHasError: KnockoutObservable<boolean>

    public PrimaryGroupNameErrorDescription: KnockoutObservable<string>
    public SecondaryGroupNameErrorDescription: KnockoutObservable<string>
    public ProjectNameErrorDescription: KnockoutObservable<string>

    public NewProjectCheckError: KnockoutObservable<boolean>

    constructor(activeProject: KnockoutObservable<OneDasProjectViewModel>)
    {
        super('editor', 'Editor', 'editor.html', activeProject)

        this.ProjectDescriptionSet = ko.observableArray<OneDasProjectDescriptionViewModel>()
        this.Project = ko.observable<OneDasProjectViewModel>()
        this.PrimaryGroupName = ko.observable<string>("")
        this.SecondaryGroupName = ko.observable<string>("")
        this.ProjectName = ko.observable<string>("")

        this.PrimaryGroupNameHasError = ko.observable<boolean>(false)
        this.SecondaryGroupNameHasError = ko.observable<boolean>(false)
        this.ProjectNameHasError = ko.observable<boolean>(false)

        this.PrimaryGroupNameErrorDescription = ko.observable<string>("")
        this.SecondaryGroupNameErrorDescription = ko.observable<string>("")
        this.ProjectNameErrorDescription = ko.observable<string>("")

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

        this.ProjectName.subscribe((value) =>
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

            this.ProjectNameHasError(result.HasError)
            this.ProjectNameErrorDescription(result.ErrorDescription)
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
            this.ProjectDescriptionSet(projectDescriptionSet.map(projectDescription => new OneDasProjectDescriptionViewModel(projectDescription)))
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
            projectModel = await ConnectionManager.InvokeWebClientHub("CreateProject", this.PrimaryGroupName(), this.SecondaryGroupName(), this.ProjectName())

            this.Project(new OneDasProjectViewModel(projectModel))

            await this.Project().InitializeAsync(projectModel.DataGatewaySettingsSet, projectModel.DataWriterSettingsSet)

            this.NewProjectCheckError(false)
            this.PrimaryGroupName("")

            this.NewProjectCheckError(false)
            this.SecondaryGroupName("")

            this.NewProjectCheckError(false)
            this.ProjectName("")
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

    public OpenProject = async (projectDescriptionViewModel: OneDasProjectDescriptionViewModel) =>
    {
        try
        {
            let projectModel: any

            projectModel = await ConnectionManager.InvokeWebClientHub("OpenProject", projectDescriptionViewModel.ToModel())
            
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