using Microsoft.Extensions.DependencyInjection;
using OneDas.Common;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OneDas.Plugin
{
    public interface IPluginProvider
    {
        #region "Hive"

        void ScanAssemblies(string directoryPath, string productName, Version minimumVersion);

        void ScanAssembly(string filePath);

        List<Type> LoadAssemblies(string directoryPath, string productName, Version minimumVersion);

        List<Type> LoadAssemby(string filePath);

        IEnumerable<Type> GetPluginsByBaseClass<TBaseClass>();

        IEnumerable<Type> Get<T>();

        Type Get<TPluginSettings>(string pluginId);

        void Add<T>(Type pluginType);

        void Add(Type baseType, Type pluginType);

        void AddRange<T>(IEnumerable<Type> pluginTypeSet);

        void AddRange(Type baseType, IEnumerable<Type> pluginTypeSet);

        void Clear<T>();

        #endregion

        #region "Factory"

        IPluginSupporter BuildSupporter(Type pluginSettingsType);

        IPluginSupporter TryBuildSupporter(Type pluginSettingsType);

        TPluginLogic BuildContainer<TPluginLogic>(PluginSettingsBase pluginSettings, params object[] args) where TPluginLogic : PluginLogicBase;

        #endregion

        #region "Helper"

        ActionResponse HandleActionRequest(ActionRequest actionRequest);

        string GetStringResource(string pluginId, string resourceName);

        #endregion
    }
}