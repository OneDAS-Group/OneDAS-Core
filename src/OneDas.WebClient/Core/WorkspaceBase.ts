class WorkspaceBase
{
    public Address: string
    public Title: string
    public ViewName: string
    public ActiveProject: KnockoutObservable<ProjectViewModel>

    constructor(address: string, title: string, viewName: string, activeProject: KnockoutObservable<ProjectViewModel>)
    {
        this.Address = address
        this.Title = title
        this.ViewName = viewName
        this.ActiveProject = activeProject
    }
}