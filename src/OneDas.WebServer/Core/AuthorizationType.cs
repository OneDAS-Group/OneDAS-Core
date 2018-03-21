namespace OneDas.WebServer.Core
{
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