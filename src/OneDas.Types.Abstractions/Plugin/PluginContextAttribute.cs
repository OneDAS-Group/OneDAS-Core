using System;

namespace OneDas.Plugin
{
    [AttributeUsage(validOn: AttributeTargets.Class, AllowMultiple = false)]
    public class PluginContextAttribute : Attribute
    {
        public PluginContextAttribute(Type pluginLogic)
        {
            this.PluginLogic = pluginLogic;
        }

        public Type PluginLogic { get; }
    }
}

