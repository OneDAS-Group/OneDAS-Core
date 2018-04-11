using System;
using System.Collections.Generic;
using System.Reflection;

namespace OneDas.Plugin
{
    public interface IPluginProvider
    {
        #region "Hive"

        Type GetSettings(string pluginId);

        Type GetLogic(string pluginId);

        IEnumerable<Type> Get<TPluginBase>();

        IEnumerable<PluginIdentificationAttribute> GetIdentifications<TPluginSettings>() where TPluginSettings : PluginSettingsBase;

        void ScanAssembly(Assembly assembly);

        void Add(Type pluginType);

        void AddRange(IEnumerable<Type> pluginTypeSet);

        void Clear();

        #endregion

        #region "Factory"

        IPluginSupporter BuildSupporter(Type pluginSettingsType);

        IPluginSupporter TryBuildSupporter(Type pluginSettingsType);

        TPluginLogic BuildLogic<TPluginLogic>(PluginSettingsBase pluginSettings, params object[] args) where TPluginLogic : PluginLogicBase;

        #endregion

        #region "Helper"

        ActionResponse HandleActionRequest(ActionRequest actionRequest);

        string GetStringResource(string pluginId, string resourceName);

        #endregion
    }
}