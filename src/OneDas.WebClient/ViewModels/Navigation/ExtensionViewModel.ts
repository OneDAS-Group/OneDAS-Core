class ExtensionViewModel extends WorkspaceBase {
    constructor(activeProject: KnockoutObservable<OneDasProjectViewModel>) {
        super('extension', 'Extensions', 'extension.html', activeProject)
    }
}