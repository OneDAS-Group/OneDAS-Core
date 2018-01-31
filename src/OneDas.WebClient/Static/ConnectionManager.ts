class ConnectionManager
{
    public static WebClientHub: any // improve: use typings

    public static Initialize(enableLogging: boolean)
    {
        ConnectionManager.WebClientHub = new signalR.HubConnection('/webclienthub');
    }

    public static InvokeWebClientHub = async(methodName: string, ...args: any[]) =>
    {
        return await Promise.resolve(ConnectionManager.WebClientHub.invoke(methodName, ...args))
    }
}
