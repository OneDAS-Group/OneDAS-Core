class PluginIdentificationViewModel
{
    public ProductVersion: string
    public Id: string
    public Name: string
    public Description: string
    public ViewResourceName: string
    public ViewModelResourceName: string

    constructor(pluginIdentificationModel: any)
    {
        this.ProductVersion = pluginIdentificationModel.ProductVersion
        this.Id = pluginIdentificationModel.Id
        this.Name = pluginIdentificationModel.Name
        this.Description = pluginIdentificationModel.Description
        this.ViewResourceName = pluginIdentificationModel.ViewResourceName
        this.ViewModelResourceName = pluginIdentificationModel.ViewModelResourceName
    }
}