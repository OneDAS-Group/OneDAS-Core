using System.Runtime.Serialization;

namespace OneDas.WebServer.PackageManagement
{
    [DataContract]
    public class PackageSearchMetadataLight
    {
        #region "Constructors"

        public PackageSearchMetadataLight(string packageId, string description, string version)
        {
            this.PackageId = packageId;
            this.Description = description;
            this.Version = version;
        }

        #endregion

        #region "Properties"

        [DataMember]
        string PackageId { get; }

        [DataMember]
        string Description { get; }

        [DataMember]
        string Version { get; }

        #endregion
    }
}
