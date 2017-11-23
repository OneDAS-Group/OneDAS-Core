using System;
using System.Reflection;

namespace OneDas.Plugin
{
    [AttributeUsage(validOn: AttributeTargets.Class, AllowMultiple = false)]
    public class PluginInitializerAttribute : Attribute
    {
        public PluginInitializerAttribute(Type type, string methodName)
        {
            this.InitializerAction = () => type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static).Invoke(null, null);
        }

        public Action InitializerAction { get; private set; }
    }
}

