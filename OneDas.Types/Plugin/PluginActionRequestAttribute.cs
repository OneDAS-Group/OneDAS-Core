using System;
using System.Diagnostics.Contracts;

namespace OneDas.Plugin
{
    [AttributeUsage(validOn: AttributeTargets.Class, AllowMultiple = false)]
    public class PluginActionRequestAttribute : Attribute
    {
        public PluginActionRequestAttribute(string methodName, Type type)
        {
            Contract.Requires(!string.IsNullOrEmpty(methodName));
            Contract.Requires(type != null);

            this.MethodName = methodName;
            this.Type = type;
        }

        public string MethodName { get; private set; }
        public Type Type { get; private set; }
    }
}

