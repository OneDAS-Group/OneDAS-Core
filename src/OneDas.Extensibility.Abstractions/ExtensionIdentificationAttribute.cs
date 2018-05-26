using System;
using System.Runtime.Serialization;

namespace OneDas.Extensibility
{
    [DataContract]
    [AttributeUsage(validOn: AttributeTargets.Class, AllowMultiple = false)]
    public class ExtensionIdentificationAttribute : Attribute
    {
        public ExtensionIdentificationAttribute(string id, string name, string description, string relativeViewPath, string relativeViewModelPath)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.ViewResourceName = relativeViewPath;
            this.ViewModelResourceName = relativeViewModelPath;

            this.ProductVersion = "N/A";
        }

        [DataMember]
        public string ProductVersion { get; set; }

        [DataMember]
        public string Id { get; }

        [DataMember]
        public string Name { get; }

        [DataMember]
        public string Description { get; }

        [DataMember]
        public string ViewResourceName { get; }

        [DataMember]
        public string ViewModelResourceName { get; }
    }
}