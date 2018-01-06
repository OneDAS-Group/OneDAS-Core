using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using OneDas.Common;

namespace OneDas.Plugin
{
    public static class PluginFactory
    {
        public static IEnumerable<TPluginSettings> BuildDefaultSettings<TPluginSettings>(Func<Type, bool> selector, params object[] args) where TPluginSettings : PluginSettingsBase
        {
            return PluginHive.GetPluginSet<TPluginSettings>().Where(selector).Select(pluginSettings => (TPluginSettings)Activator.CreateInstance(pluginSettings, args)).ToList();
        }

        public static IEnumerable<TPluginLogic> BuildSettingsContainers<TPluginLogic>(IEnumerable<PluginSettingsBase> pluginSettingsSet, ILoggerFactory loggerFactory) where TPluginLogic : PluginLogicBase
        {
            return pluginSettingsSet.ToList().Select(pluginSettings =>
            {
                Type pluginLogicType = pluginSettings.GetType().GetFirstAttribute<PluginContextAttribute>().PluginLogic;

                if (pluginLogicType == null)
                {
                    throw new Exception(ErrorMessage.PluginFactory_NoMatchingTPluginLogicFound);
                }

                return (TPluginLogic)Activator.CreateInstance(pluginLogicType, pluginSettings, loggerFactory);
            }).ToList();
        }

        public static TPluginSettings BuildSettings<TPluginSettings>(string pluginName) where TPluginSettings : PluginSettingsBase
        {
            IEnumerable<Type> pluginSettingsTypeSet = PluginHive.GetPluginSet<TPluginSettings>();
            Type pluginSettingsType = pluginSettingsTypeSet.ToList().FirstOrDefault(x => x.GetFirstAttribute<PluginIdentificationAttribute>().Name == pluginName);

            if (pluginSettingsType == null)
            {
                throw new Exception(ErrorMessage.PluginFactory_NoMatchingTPluginSettingsFound);
            }

            return (TPluginSettings)Activator.CreateInstance(pluginSettingsType);
        }
    }
}
