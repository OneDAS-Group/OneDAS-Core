class PackageMetadataViewModel
{
    public PackageId: KnockoutObservable<string>
    public Description: KnockoutObservable<string>
    public Version: KnockoutObservable<string>
    public IsInstalled: KnockoutObservable<boolean>
    public IsUpdateAvailable: KnockoutObservable<boolean>

    constructor(packageMetadataModel: any)
    {
        this.PackageId = ko.observable<string>(packageMetadataModel.PackageId)
        this.Description = ko.observable<string>(packageMetadataModel.Description)
        this.Version = ko.observable<string>(packageMetadataModel.Version)
        this.IsInstalled = ko.observable<boolean>(packageMetadataModel.IsInstalled)
        this.IsUpdateAvailable = ko.observable<boolean>(packageMetadataModel.IsUpdateAvailable)
    }
}