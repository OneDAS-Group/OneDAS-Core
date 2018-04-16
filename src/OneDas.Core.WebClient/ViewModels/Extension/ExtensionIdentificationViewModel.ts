class ExtensionIdentificationViewModel
{
    public ProductVersion: string
    public Id: string
    public Name: string
    public Description: string
    public ViewResourceName: string
    public ViewModelResourceName: string

    constructor(extensionIdentificationModel: any)
    {
        this.ProductVersion = extensionIdentificationModel.ProductVersion
        this.Id = extensionIdentificationModel.Id
        this.Name = extensionIdentificationModel.Name
        this.Description = extensionIdentificationModel.Description
        this.ViewResourceName = extensionIdentificationModel.ViewResourceName
        this.ViewModelResourceName = extensionIdentificationModel.ViewModelResourceName
    }
}