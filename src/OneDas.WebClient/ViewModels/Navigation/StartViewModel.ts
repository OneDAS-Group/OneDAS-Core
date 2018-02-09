class StartViewModel extends WorkspaceBase
{
    constructor(activeProject: KnockoutObservable<OneDasProjectViewModel>)
    {
        super('start', 'Start', 'start.html', activeProject)
    }
}