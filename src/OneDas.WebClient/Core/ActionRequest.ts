class ActionRequest
{
    public readonly PluginId: string
    public readonly InstanceId: number
    public readonly MethodName: string
    public readonly Data: any

    constructor(pluginId: string, instanceId: number, methodName: string, data: any)
    {
        this.PluginId = pluginId;
        this.InstanceId = instanceId;
        this.MethodName = methodName;
        this.Data = data;
    }
}