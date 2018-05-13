declare var signalR: any

class ConnectionManager
{
    public static WebClientHub: any // improve: use typings

    public static Initialize(enableLogging: boolean)
    {
        ConnectionManager.WebClientHub = new signalR.HubConnectionBuilder()
                                                .configureLogging(signalR.LogLevel.Information)
                                                .withUrl('/webclienthub')
                                                .build();
    }

    public static InvokeWebClientHub = async(methodName: string, ...args: any[]) =>
    {
        return await Promise.resolve(ConnectionManager.WebClientHub.invoke(methodName, ...args))
    }
}
