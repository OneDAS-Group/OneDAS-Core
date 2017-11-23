class ExtensionViewModel extends WorkspaceBase {
    constructor(activeProject: KnockoutObservable<ProjectViewModel>) {
        super('extension', 'Extensions', 'extension.html', activeProject)
    }
}