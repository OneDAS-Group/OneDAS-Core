using System;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace OneDas.Extensibility
{
    [DataContract]
    public class ExtensionDescription
    {
        public ExtensionDescription(string productVersion, string id, bool isEnabled)
        {
            this.ProductVersion = productVersion;
            this.Id = id;
            this.IsEnabled = isEnabled;

            this.InstanceName = string.Empty;
        }

        [DataMember]
        public string ProductVersion { get; private set; }

        [DataMember]
        public string Id { get; private set; }

        [DataMember]
        public int InstanceId { get; set; }

        [DataMember]
        public string InstanceName { get; set; }

        [DataMember]
        public bool IsEnabled { get; private set; }

        public void Validate()
        {
            Contract.Requires(this.ProductVersion != null);

            string errorMessage;

            if (!OneDasUtilities.CheckNamingConvention(this.Id, out errorMessage))
            {
                throw new Exception($"The ID is invalid: { errorMessage }");
            }
        }
    }
}