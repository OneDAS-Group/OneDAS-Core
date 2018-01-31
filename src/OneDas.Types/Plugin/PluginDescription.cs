using OneDas.Infrastructure;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace OneDas.Plugin
{
    [DataContract]
    public class PluginDescription
    {
        public PluginDescription(string productVersion, string id, bool isEnabled)
        {
            this.ProductVersion = productVersion;
            this.Id = id;
            this.IsEnabled = isEnabled;
        }

        [DataMember]
        public string ProductVersion { get; private set; }

        [DataMember]
        public string Id { get; private set; }

        [DataMember]
        public int InstanceId { get; internal set; }

        [DataMember]
        public bool IsEnabled { get; private set; }

        public void Validate()
        {
            Contract.Requires(this.ProductVersion != null);

            string errorMessage;

            if (!InfrastructureHelper.CheckNamingConvention(this.Id, out errorMessage))
            {
                throw new ValidationException($"The ID is invalid: { errorMessage }");
            }
        }
    }
}