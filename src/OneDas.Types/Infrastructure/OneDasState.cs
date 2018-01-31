namespace OneDas.Infrastructure
{
    /// <summary>
    /// Specifies the state of the OneDAS server.
    /// </summary>
    public enum OneDasState
    {
        /// <summary>
        /// An error occured which must be acknowledged. The OneDAS server will then try to reinitialize itself.
        /// </summary>
        Error = 1,

        /// <summary>
        /// The OneDAS server initalizes the WCF endpoint to listen for clients and loads the configuration if available. Otherwise a new empty configuration is created.
        /// </summary>
        Initialization = 2,

        /// <summary>
        /// The OneDAS server initialized sucessfully but TwinCAT is not configured yet.
        /// </summary>
        Unconfigured = 3,

        /// <summary>
        /// The OneDAS server applies the configuration to TwinCAT and establishes a connection with TwinCAT to exchange data.
        /// </summary>
        ApplyConfiguration = 5,

        /// <summary>
        /// The OneDAS server successfully applied the configuration or was stopped.
        /// </summary>
        Ready = 6,

        /// <summary>
        /// The OneDAS server gathers and stores the data depending on the configuration.
        /// </summary>
        Run = 7
    }
}