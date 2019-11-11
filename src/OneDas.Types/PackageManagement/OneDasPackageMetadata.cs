using System.Runtime.Serialization;

namespace OneDas.PackageManagement
{
    [DataContract]
    public class OneDasPackageMetaData
    {
        #region "Constructors"

        public OneDasPackageMetaData(string packageId, string description, string version, bool isInstalled, bool isUpdateAvailable)
        {
            this.PackageId = packageId;
            this.Description = description;
            this.Version = version;
            this.IsInstalled = isInstalled;
            this.IsUpdateAvailable = isUpdateAvailable;
        }

        #endregion

        #region "Properties"

        [DataMember]
        string PackageId { get; }

        [DataMember]
        string Description { get; }

        [DataMember]
        string Version { get; }

        [DataMember]
        bool IsInstalled { get; }

        [DataMember]
        bool IsUpdateAvailable { get; }

        #endregion
    }
}
