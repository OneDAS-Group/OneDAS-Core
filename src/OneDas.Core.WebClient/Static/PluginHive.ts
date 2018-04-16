class PluginHive
{
    // fields
    public static PluginIdentificationSet: Map<string, PluginIdentificationViewModel[]>

    // constructors
    static Initialize = () =>
    {
        PluginHive.PluginIdentificationSet = new Map<string, PluginIdentificationViewModel[]>()
    }   

    static FindPluginIdentification = (pluginTypeName: string, pluginId: string) =>
    {
        return PluginHive.PluginIdentificationSet.get(pluginTypeName).find(pluginIdentification => pluginIdentification.Id === pluginId);
    }
}