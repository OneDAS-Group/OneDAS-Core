class ExtensionDescriptionViewModel
{
    public ProductVersion: number
    public Id: string
    public InstanceId: number
    public InstanceName: KnockoutObservable<string>
    public IsEnabled: KnockoutObservable<boolean>

    constructor(extensionDescriptionModel: any)
    {
        this.ProductVersion = extensionDescriptionModel.ProductVersion
        this.Id = extensionDescriptionModel.Id
        this.InstanceId = extensionDescriptionModel.InstanceId
        this.InstanceName = ko.observable<string>(extensionDescriptionModel.InstanceName)
        this.IsEnabled = ko.observable<boolean>(extensionDescriptionModel.IsEnabled)
    }

    public ToModel()
    {
        var model: any = {
            ProductVersion: <number>this.ProductVersion,
            Id: <string>this.Id,
            InstanceId: <number>this.InstanceId,
            InstanceName: <string>this.InstanceName(),
            IsEnabled: <boolean>this.IsEnabled()
        }

        return model
    }
}