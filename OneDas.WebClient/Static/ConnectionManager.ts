class ConnectionManager
{
    public static Broadcaster: any // improve: use typings

    public static Initialize(enableLogging: boolean)
    {
        ConnectionManager.Broadcaster = new signalR.HubConnection('/broadcaster');
    }

    public static InvokeBroadcaster = async(methodName: string, ...args: any[]) =>
    {
        return await Promise.resolve(ConnectionManager.Broadcaster.invoke(methodName, ...args))
    }
}
