class ControlViewModel extends WorkspaceBase
{
    public ProjectDescriptionSet: KnockoutObservableArray<OneDasProjectDescriptionViewModel>

    constructor(activeProject: KnockoutObservable<OneDasProjectViewModel>)
    {
        super('control', 'Control', 'control.html', activeProject)

        this.ProjectDescriptionSet = ko.observableArray<OneDasProjectDescriptionViewModel>()
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

    public ActivateProject = async (projectDescriptionViewModel: OneDasProjectDescriptionViewModel) =>
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

    public PauseOneDas = async () => {
        try {
            await ConnectionManager.InvokeWebClientHub("PauseOneDas")
        }
        catch (e) {
            alert(e.message)
        }
    }

    public StopOneDas = async () =>
    {
        try
        {
            if (confirm("OneDAS engine will be stopped.")) {

                await ConnectionManager.InvokeWebClientHub("StopOneDas")

                this.ActiveProject(null)
            }
        }
        catch (e)
        {
            alert(e.message)
        }
    }
}