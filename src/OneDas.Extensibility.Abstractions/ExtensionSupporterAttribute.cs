using System;
using System.Diagnostics.Contracts;

namespace OneDas.Extensibility
{
    [AttributeUsage(validOn: AttributeTargets.Class, AllowMultiple = false)]
    public class ExtensionSupporterAttribute : Attribute
    {
        public ExtensionSupporterAttribute(Type type)
        {
            Contract.Requires(type != null);

            this.Type = type;
        }

        public Type Type { get; private set; }
    }
}