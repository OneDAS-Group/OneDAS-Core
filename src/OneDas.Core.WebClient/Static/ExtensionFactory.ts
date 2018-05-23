class ExtensionFactory
{
    public static CreateExtensionViewModelAsync = async (extensionType: string, extensionModel: any) =>
    {
        let extensionIdentification: ExtensionIdentificationViewModel
        let extensionViewModel: ExtensionViewModelBase
        let extensionViewModelRaw: string

        extensionIdentification = ExtensionHive.FindExtensionIdentification(extensionType, extensionModel.Description.Id)

        if (extensionIdentification)
        {
            extensionViewModelRaw = await ConnectionManager.InvokeWebClientHub("GetExtensionStringResource", extensionModel.Description.Id, extensionIdentification.ViewModelResourceName)
            extensionViewModel = <ExtensionViewModelBase>new Function(extensionViewModelRaw + "; return ViewModelConstructor")()(extensionModel, extensionIdentification)

            return extensionViewModel
        }
        else
        {
            throw new Error("No corresponding extension description for extension ID '" + extensionModel.Description.Id + "' found.")
        }
    }
}