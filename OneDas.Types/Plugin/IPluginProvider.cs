using System;
using System.Collections.Generic;

namespace OneDas.Plugin
{
    public interface IPluginProvider
    {
        #region "Hive"

        void ScanAssemblies(string directoryPath, string productName, Version minimumVersion);

        void ScanAssembly(string filePath);

        List<Type> LoadAssemblies(string directoryPath, string productName, Version minimumVersion);

        List<Type> LoadAssemby(string filePath);

        Type GetSettings(string pluginId);

        Type GetLogic(string pluginId);

        IEnumerable<Type> Get<TPluginBase>();

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