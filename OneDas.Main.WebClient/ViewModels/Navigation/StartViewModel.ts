class StartViewModel extends WorkspaceBase
{
    constructor(activeProject: KnockoutObservable<ProjectViewModel>)
    {
        super('start', 'Start', 'start.html', activeProject)
    }
}