using System;
using System.Diagnostics.Contracts;

namespace OneDas.Plugin
{
    [AttributeUsage(validOn: AttributeTargets.Class, AllowMultiple = false)]
    public class PluginSupporterAttribute : Attribute
    {
        public PluginSupporterAttribute(Type type)
        {
            Contract.Requires(type != null);

            this.Type = type;
        }

        public Type Type { get; private set; }
    }
}