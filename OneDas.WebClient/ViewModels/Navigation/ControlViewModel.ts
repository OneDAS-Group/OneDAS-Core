class ControlViewModel extends WorkspaceBase
{
    public ProjectDescriptionSet: KnockoutObservableArray<ProjectDescriptionViewModel>

    constructor(activeProject: KnockoutObservable<ProjectViewModel>)
    {
        super('control', 'Control', 'control.html', activeProject)

        this.ProjectDescriptionSet = ko.observableArray<ProjectDescriptionViewModel>()
    }

    // commands
    public GetProjectDescriptions = async () =>
    {
        let projectDescriptionSet: any[]

        try
        {
            projectDescriptionSet = await ConnectionManager.InvokeBroadcaster('GetProjectDescriptions')
            this.ProjectDescriptionSet(projectDescriptionSet.map(projectDescription => new ProjectDescriptionViewModel(projectDescription)))
        }
        catch (e)
        {
            alert(e.message)
        }
    }

    public ActivateProject = async (projectDescriptionViewModel: ProjectDescriptionViewModel) =>
    {
        try
        {
            await ConnectionManager.InvokeBroadcaster("ActivateProject", projectDescriptionViewModel.ToModel())
        }
        catch (e)
        {
            alert(e.message)
        }
    }

    public StartOneDas = async () =>
    {
        try
        {
            await ConnectionManager.InvokeBroadcaster("StartOneDas")
        }
        catch (e)
        {
            alert(e.message)
        }
    }

    public StopOneDas = async () =>
    {
        try
        {
            await ConnectionManager.InvokeBroadcaster("StopOneDas")
        }
        catch (e)
        {
            alert(e.message)
        }
    }
}