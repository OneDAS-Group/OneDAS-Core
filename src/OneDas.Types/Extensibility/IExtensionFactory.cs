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

        IEnumerable<Type> Get<TExtensionBase>();

        IEnumerable<ExtensionIdentificationAttribute> GetIdentifications<TExtensionSettings>() where TExtensionSettings : ExtensionSettingsBase;

        void ScanAssembly(Assembly assembly);

        void Add(Type type);

        void AddRange(IEnumerable<Type> typeSet);

        void Clear();

        #endregion

        #region "Factory"

        IExtensionSupporter BuildSupporter(Type settingsType);

        IExtensionSupporter TryBuildSupporter(Type settingsType);

        TExtensionLogic BuildLogic<TExtensionLogic>(ExtensionSettingsBase settings, params object[] args) where TExtensionLogic : ExtensionLogicBase;

        #endregion

        #region "Helper"

        ActionResponse HandleActionRequest(ActionRequest actionRequest);

        string GetStringResource(string id, string resourceName);

        #endregion
    }
}