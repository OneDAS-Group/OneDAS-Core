class ExtensionViewModel extends WorkspaceBase
{
    public SearchTerm: KnockoutObservable<string>
    public PackageSearchMetadataSet: KnockoutObservableArray<any>
    public SelectedPackageSource: KnockoutObservable<OneDasPackageSourceViewModel>
    public IsSearching: KnockoutObservable<boolean>

    constructor(activeProject: KnockoutObservable<OneDasProjectViewModel>)
    {
        super('extension', 'Extensions', 'extension.html', activeProject)

        this.SearchTerm = ko.observable<string>("").extend({ rateLimit: { timeout: 500, method: "notifyWhenChangesStop" } });
        this.PackageSearchMetadataSet = ko.observableArray([])
        this.SelectedPackageSource = ko.observable<OneDasPackageSourceViewModel>()
        this.IsSearching = ko.observable<boolean>(false)

        // search
        this.SearchTerm.subscribe(newValue =>
        {
            this.SearchPlugins(this.SearchTerm())
        })

        this.SelectedPackageSource.subscribe(newValue =>
        {
            if (newValue)
            {
                this.SearchPlugins(this.SearchTerm())
            }
            else
            {
                this.PackageSearchMetadataSet([])
            }
        })
    }

    // method
    public SearchPlugins = async (searchTerm: string) =>
    {
        let packageSource: OneDasPackageSourceViewModel

        if (this.SelectedPackageSource())
        {
            packageSource = this.SelectedPackageSource()

            try
            {
                this.IsSearching(true)
                this.PackageSearchMetadataSet(await ConnectionManager.InvokeWebClientHub("SearchPlugins", searchTerm, packageSource.Address))
            } 
            finally
            {
                this.IsSearching(false)
            }
        }
    }

    // command
    public InstallPlugin = async (packageSearchMetaData) =>
    {
        try
        {
            await ConnectionManager.InvokeWebClientHub("InstallPlugin", packageSearchMetaData.PackageId)
        }
        catch (e)
        {
            alert(e.message)
        }
    }

    public UpdatePlugin = async (packageSearchMetaData) =>
    {
        try
        {
            await ConnectionManager.InvokeWebClientHub("UpdatePlugin", packageSearchMetaData.PackageId)
        }
        catch (e)
        {
            alert(e.message)
        }
    }

    public UninstallPlugin = async (packageSearchMetaData) =>
    {
        try
        {
            await ConnectionManager.InvokeWebClientHub("UninstallPlugin", packageSearchMetaData.PackageId)
        }
        catch (e)
        {
            alert(e.message)
        }
    }

    public ResetSearch = () =>
    {
        this.SearchTerm("")
    }
}