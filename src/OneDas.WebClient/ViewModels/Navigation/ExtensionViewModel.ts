class ExtensionViewModel extends WorkspaceBase
{
    public SearchTerm: KnockoutObservable<string>
    public SearchPackageMetadataSet: KnockoutObservableArray<PackageMetadataViewModel>
    public SelectedPackageSource: KnockoutObservable<OneDasPackageSourceViewModel>
    public IsSearching: KnockoutObservable<boolean>

    constructor(activeProject: KnockoutObservable<OneDasProjectViewModel>)
    {
        super('extension', 'Extensions', 'extension.html', activeProject)

        this.SearchTerm = ko.observable<string>("").extend({ rateLimit: { timeout: 500, method: "notifyWhenChangesStop" } });
        this.SearchPackageMetadataSet = ko.observableArray<PackageMetadataViewModel>()
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
                this.SearchPackageMetadataSet([])
            }
        })
    }

    // method
    public SearchPlugins = async (searchTerm: string) =>
    {
        let packageSource: OneDasPackageSourceViewModel
        let packageMetaDataSet: any[]

        if (this.SelectedPackageSource())
        {
            packageSource = this.SelectedPackageSource()

            try
            {
                this.IsSearching(true)

                packageMetaDataSet = await ConnectionManager.InvokeWebClientHub("SearchPlugins", searchTerm, packageSource.Address)
                this.SearchPackageMetadataSet(packageMetaDataSet.map(packageMetaData => new PackageMetadataViewModel(packageMetaData)))
            } 
            finally
            {
                this.IsSearching(false)
            }
        }
    }

    // command
    public InstallPlugin = async (packageMetaData) =>
    {
        try
        {
            await ConnectionManager.InvokeWebClientHub("InstallPlugin", packageMetaData.PackageId(), this.SelectedPackageSource().Address)
        }
        catch (e)
        {
            alert(e.message)
        }
    }

    public UpdatePlugin = async (packageMetaData) =>
    {
        try
        {
            await ConnectionManager.InvokeWebClientHub("UpdatePlugin", packageMetaData.PackageId())
        }
        catch (e)
        {
            alert(e.message)
        }
    }

    public UninstallPlugin = async (packageMetaData) =>
    {
        try
        {
            await ConnectionManager.InvokeWebClientHub("UninstallPlugin", packageMetaData.PackageId())
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