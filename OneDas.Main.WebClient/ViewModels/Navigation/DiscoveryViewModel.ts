class DiscoveryViewModel extends WorkspaceBase {
    constructor(activeProject: KnockoutObservable<ProjectViewModel>) {
        super('discovery', 'Discovery', 'discovery.html', activeProject)
    }
}