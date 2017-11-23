using System.Collections.Generic;

namespace OneDas.Infrastructure
{
    /// <summary>
    /// Represents a container which is transmitted from the OneDAS server to the OneDAS client during the initial communication.
    /// </summary>
    public struct WcfClientStarterPackage
    {
        #region "Constructors"

        /// <summary>
        /// Initializes a new instance of the structure WcfClientStarterPackage with the provided OneDAS state, the last error, a list of registered clients and the active client.
        /// </summary>
        /// <param name="oneDasName">The <see cref="OneDasName"/>.</param>
        /// <param name="oneDasState">The <see cref="OneDasState"/>.</param>
        /// <param name="lastError">The last error logged by the OneDAS server.</param>
        /// <param name="clientSet">The list of <see cref="WcfClientDescription"/>.</param>
        /// <param name="activeClient">The active <see cref="WcfClientDescription"/>.</param>
        public WcfClientStarterPackage(string oneDasName, OneDasState oneDasState, string lastError, IEnumerable<WcfClientDescription> clientSet, WcfClientDescription activeClient)
        {
            this.OneDasName = oneDasName;
            this.OneDasState = oneDasState;
            this.LastError = lastError;
            this.ClientSet = clientSet;
            this.ActiveClient = activeClient;
        }

        #endregion

        #region "Properties"

        /// <summary>
        /// Gets or sets the current OneDAS name.
        /// </summary>
        /// <returns>Returns the current OneDAS name.</returns>
        public string OneDasName { get; set; }

        /// <summary>
        /// Gets or sets the current OneDAS state.
        /// </summary>
        /// <returns>Returns the current OneDAS state.</returns>
        public OneDasState OneDasState { get; set; }

        /// <summary>
        /// Gets or sets the last error.
        /// </summary>
        /// <returns>Returns the last error.</returns>
        public string LastError { get; set; }

        /// <summary>
        /// Gets or sets the list of registered clients.
        /// </summary>
        /// <returns>Returns the list of registered clients.</returns>
        public IEnumerable<WcfClientDescription> ClientSet { get; set; }

        /// <summary>
        /// Gets or sets the active client.
        /// </summary>
        /// <returns>Returns the active client.</returns>
        public WcfClientDescription ActiveClient { get; set; }

        #endregion
    }
}