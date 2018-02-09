class DiscoveryViewModel extends WorkspaceBase {
    constructor(activeProject: KnockoutObservable<OneDasProjectViewModel>) {
        super('discovery', 'Discovery', 'discovery.html', activeProject)
    }
}