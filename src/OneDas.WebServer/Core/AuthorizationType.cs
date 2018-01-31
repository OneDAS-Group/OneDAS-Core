namespace OneDas.WebServer.Core
{
    /// <summary>
    /// Specifies the authorization type for functions called by the OneDAS client on the OneDAS server.
    /// </summary>
    public enum AuthorizationType
    {
        /// <summary>
        /// No authorization required.
        /// </summary>
        None = 0,

        /// <summary>
        /// The OneDAS client must be registered to get authorized.
        /// </summary>
        Registered = 1,

        /// <summary>
        /// The OneDAS client must own the active status to get authorized.
        /// </summary>
        Active = 2
    }
}