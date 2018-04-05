using System.Runtime.Serialization;

namespace OneDas.WebServer.PackageManagement
{
    [DataContract]
    public class OneDasPackageSource
    {
        #region "Properties"

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Address { get; set; }

        #endregion
    }
}
