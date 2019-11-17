class ExtensionViewModel extends WorkspaceBase
{
    public SearchTerm: KnockoutObservable<string>
    public SearchPackageMetadataSet: KnockoutObservableArray<PackageMetadataViewModel>
    public SelectedPackageSource: KnockoutObservable<OneDasPackageSourceViewModel>
    public IsSearching: KnockoutObservable<boolean>
    public IsProcessing: KnockoutObservable<boolean>
    public MessageLog: KnockoutObservableArray<string>
    public Skip: KnockoutObservable<number>
    public Take: KnockoutObservable<number>
    public CanClickPrevious: KnockoutComputed<boolean>
    public CanClickNext: KnockoutComputed<boolean>

    constructor(activeProject: KnockoutObservable<OneDasProjectViewModel>)
    {
        super('extension', 'Extensions', 'extension.html', activeProject)

        this.SearchTerm = ko.observable<string>("").extend({ rateLimit: { timeout: 500, method: "notifyWhenChangesStop" } });
        this.SearchPackageMetadataSet = ko.observableArray<PackageMetadataViewModel>()
        this.SelectedPackageSource = ko.observable<OneDasPackageSourceViewModel>()
        this.IsSearching = ko.observable<boolean>(false)
        this.IsProcessing = ko.observable<boolean>(false)
        this.MessageLog = ko.observableArray<string>()

        this.Skip = ko.observable<number>(0);
        this.Take = ko.observable<number>(15);

        // search
        this.CanClickPrevious = ko.pureComputed(() =>
        {
            return this.Skip() > 0
        })

        this.CanClickNext = ko.pureComputed(() =>
        {
            return this.SearchPackageMetadataSet().length == this.Take();
        })

        this.SearchTerm.subscribe(newValue =>
        {
            this.Skip(0);
            this.SearchPackages(this.SearchTerm())
        })

        this.SelectedPackageSource.subscribe(newValue =>
        {
            this.Skip(0);

            if (newValue)
            {
                this.SearchPackages(this.SearchTerm())
            }
            else
            {
                this.SearchPackageMetadataSet([])
            }
        })

        ConnectionManager.WebClientHub.on("SendNugetMessage", nugetMessage =>
        {
            this.MessageLog.unshift(nugetMessage)
        })
    }

    // method
    public SearchPackages = async (searchTerm: string) =>
    {
        let packageSource: OneDasPackageSourceViewModel
        let packageMetaDataSet: any[]

        if (this.SelectedPackageSource())
        {
            packageSource = this.SelectedPackageSource()

            try
            {
                this.IsSearching(true)
                packageMetaDataSet = await ConnectionManager.InvokeWebClientHub("SearchPackages", searchTerm, packageSource.Address, this.Skip(), this.Take())
                this.SearchPackageMetadataSet(packageMetaDataSet.map(packageMetaData => new PackageMetadataViewModel(packageMetaData)))
            } 
            catch (e)
            {
                alert(e.message)
            }
            finally
            {
                this.IsSearching(false)
            }
        }
    }

    // command
    public InstallPackage = async (packageMetaData) =>
    {
        try
        {
            this.IsProcessing(true)
            this.MessageLog.removeAll()

            await ConnectionManager.InvokeWebClientHub("InstallPackage", packageMetaData.PackageId())
            await this.SearchPackages(this.SearchTerm())
        }
        catch (e)
        {
            alert(e.message)
        }
        finally
        {
            this.IsProcessing(false)
        }
    }

    public UpdatePackage = async (packageMetaData) =>
    {
        try
        {
            this.IsProcessing(true)
            this.MessageLog.removeAll()

            await ConnectionManager.InvokeWebClientHub("UpdatePackage", packageMetaData.PackageId())
            await this.SearchPackages(this.SearchTerm())
        }
        catch (e)
        {
            alert(e.message)
        }
        finally
        {
            this.IsProcessing(false)
        }
    }

    public UninstallPackage = async (packageMetaData) =>
    {
        try
        {
            this.IsProcessing(true)
            this.MessageLog.removeAll()

            await ConnectionManager.InvokeWebClientHub("UninstallPackage", packageMetaData.PackageId())
            await this.SearchPackages(this.SearchTerm())
        }
        catch (e)
        {
            alert(e.message)
        }
        finally
        {
            this.IsProcessing(false)
        }
    }

    public PreviousPage = async () =>
    {
        try
        {
            this.Skip(math.max(0, this.Skip() - this.Take()))
            await this.SearchPackages(this.SearchTerm())
        }
        catch (e)
        {
            alert(e.message)
        }
    }

    public NextPage = async () =>
    {
        try
        {
            this.Skip(this.Skip() + this.Take())
            await this.SearchPackages(this.SearchTerm())
        }
        catch (e)
        {
            alert(e.message)
        }
    }
}