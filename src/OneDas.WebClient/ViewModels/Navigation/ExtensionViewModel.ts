class ExtensionViewModel extends WorkspaceBase
{
    public SearchString: KnockoutObservable<string>
    public SearchPackageMetadataSet: KnockoutObservableArray<any>

    constructor(activeProject: KnockoutObservable<OneDasProjectViewModel>)
    {
        super('extension', 'Extensions', 'extension.html', activeProject)

        this.SearchString = ko.observable<string>("").extend({ rateLimit: { timeout: 500, method: "notifyWhenChangesStop" } });
        this.SearchPackageMetadataSet = ko.observableArray([])

        // search
        this.SearchString.subscribe(newValue =>
        {
            this.SearchPlugins(this.SearchString())
        })
    }

    // method
    public SearchPlugins = async (searchString: string) =>
    {
        this.SearchPackageMetadataSet(await ConnectionManager.InvokeWebClientHub("SearchPlugins", searchString))
    }

    // command
    public ResetSearch = () =>
    {
        this.SearchString("")
    }
}