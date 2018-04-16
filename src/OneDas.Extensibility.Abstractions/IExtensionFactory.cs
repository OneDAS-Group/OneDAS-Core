using System;
using System.Collections.Generic;
using System.Reflection;

namespace OneDas.Extensibility
{
    public interface IExtensionFactory
    {
        #region "Hive"

        Type GetSettings(string id);

        Type GetLogic(string id);

        IEnumerable<Type> Get<TPluginBase>();

        IEnumerable<ExtensionIdentificationAttribute> GetIdentifications<TPluginSettings>() where TPluginSettings : ExtensionSettingsBase;

        void ScanAssembly(Assembly assembly);

        void Add(Type type);

        void AddRange(IEnumerable<Type> typeSet);

        void Clear();

        #endregion

        #region "Factory"

        IExtensionSupporter BuildSupporter(Type settingsType);

        IExtensionSupporter TryBuildSupporter(Type settingsType);

        TPluginLogic BuildLogic<TPluginLogic>(ExtensionSettingsBase settings, params object[] args) where TPluginLogic : ExtensionLogicBase;

        #endregion

        #region "Helper"

        ActionResponse HandleActionRequest(ActionRequest actionRequest);

        string GetStringResource(string id, string resourceName);

        #endregion
    }
}