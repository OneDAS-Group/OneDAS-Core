using System;
using System.Runtime.Serialization;

namespace OneDas.Infrastructure
{
    /// <summary>
    /// Represents a client that is currently registered at the OneDAS server.
    /// </summary>
    [DataContract]
    public class WcfClientDescription
    {
        #region "Constructors"

        /// <summary>
        /// Initializes a new instance of the WcfClientDescription class with the provided display name and new GUID.
        /// </summary>
        /// <param name="displayName">The display name of the client.</param>
        public WcfClientDescription(string displayName)
        {
            this.Guid = Guid.NewGuid();
            this.DisplayName = displayName;
        }

        #endregion

        #region "Properties"

        /// <summary>
        /// The GUID of the client.
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public Guid Guid { get; private set; }

        /// <summary>
        /// The display name of the client.
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public string DisplayName { get; private set; }

        #endregion
    }
}