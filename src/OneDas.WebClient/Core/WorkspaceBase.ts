class WorkspaceBase
{
    public Address: string
    public Title: string
    public ViewName: string
    public ActiveProject: KnockoutObservable<OneDasProjectViewModel>

    constructor(address: string, title: string, viewName: string, activeProject: KnockoutObservable<OneDasProjectViewModel>)
    {
        this.Address = address
        this.Title = title
        this.ViewName = viewName
        this.ActiveProject = activeProject
    }
}