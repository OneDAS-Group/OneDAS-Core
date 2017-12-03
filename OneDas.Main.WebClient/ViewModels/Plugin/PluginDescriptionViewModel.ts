class PluginDescriptionViewModel
{
    public ProductVersion: number
    public Id: string
    public InstanceId: number
    public IsEnabled: KnockoutObservable<boolean>

    constructor(pluginDescriptionModel: any)
    {
        this.ProductVersion = pluginDescriptionModel.ProductVersion
        this.Id = pluginDescriptionModel.Id
        this.InstanceId = pluginDescriptionModel.InstanceId
        this.IsEnabled = ko.observable<boolean>(pluginDescriptionModel.IsEnabled)
    }

    public ToModel()
    {
        var model: any = {
            ProductVersion: <number>this.ProductVersion,
            Id: <string>this.Id,
            InstanceId: <number>this.InstanceId,
            IsEnabled: <boolean>this.IsEnabled()
        }

        return model
    }
}