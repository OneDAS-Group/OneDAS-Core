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
            projectDescriptionSet = await ConnectionManager.InvokeWebClientHub('GetProjectDescriptions')
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
            await ConnectionManager.InvokeWebClientHub("ActivateProject", projectDescriptionViewModel.ToModel())
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
            await ConnectionManager.InvokeWebClientHub("StartOneDas")
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
            await ConnectionManager.InvokeWebClientHub("StopOneDas")
        }
        catch (e)
        {
            alert(e.message)
        }
    }
}