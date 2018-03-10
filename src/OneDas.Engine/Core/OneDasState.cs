namespace OneDas.Infrastructure
{
    /// <summary>
    /// Specifies the state of the OneDAS server.
    /// </summary>
    public enum OneDasState
    {
        Error = 1,
        Initialization = 2,
        Idle = 3,
        ApplyConfiguration = 4,
        Ready = 5,
        Run = 6
    }
}