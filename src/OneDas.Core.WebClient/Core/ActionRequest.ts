class ActionRequest
{
    public readonly ExtensionId: string
    public readonly InstanceId: number
    public readonly MethodName: string
    public readonly Data: any

    constructor(extensionId: string, instanceId: number, methodName: string, data: any)
    {
        this.ExtensionId = extensionId;
        this.InstanceId = instanceId;
        this.MethodName = methodName;
        this.Data = data;
    }
}