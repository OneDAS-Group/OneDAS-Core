class PluginFactory
{
    public static CreatePluginViewModelAsync = async (pluginType: string, pluginModel: any) =>
    {
        let pluginIdentification: PluginIdentificationViewModel
        let pluginViewModel: PluginViewModelBase
        let pluginViewModelRaw: string

        pluginIdentification = PluginHive.FindPluginIdentification(pluginType, pluginModel.Description.Id)

        if (pluginIdentification)
        {
            pluginViewModelRaw = await ConnectionManager.InvokeBroadcaster("GetPluginStringResource", pluginModel.Description.Id, pluginIdentification.ViewModelResourceName)
            pluginViewModel = <PluginViewModelBase>new Function(pluginViewModelRaw + "; return ViewModelConstructor")()(pluginModel, pluginIdentification)

            return pluginViewModel
        }
        else
        {
            throw new Error("No corresponding plugin description found.")
        }
    }
}