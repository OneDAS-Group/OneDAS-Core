using Microsoft.Extensions.DependencyInjection;
using OneDas.Common;
using OneDas.Engine.Serialization;
using OneDas.Infrastructure;
using OneDas.Plugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OneDas.Engine.Core
{
    public class PluginProvider : IPluginProvider
    {
        #region "Fields"

        private IServiceProvider _serviceProvider;
        private Dictionary<Type, HashSet<Type>> _pluginDictionary;

        #endregion

        #region "Constructors"

        public PluginProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _pluginDictionary = new Dictionary<Type, HashSet<Type>>();
        }

        #endregion

        #region "Hive"

        public void ScanAssemblies(string directoryPath, string productName, Version minimumVersion)
        {
            List<Type> typeSet;

            typeSet = this.LoadAssemblies(directoryPath, productName, minimumVersion);
            this.InternalScanAssembly(typeSet);
        }

        public void ScanAssembly(string filePath)
        {
            List<Type> typeSet;

            typeSet = this.LoadAssemby(filePath);
            this.InternalScanAssembly(typeSet);
        }

        public List<Type> LoadAssemblies(string directoryPath, string productName, Version minimumVersion)
        {
            List<string> dllFilePathSet;

            dllFilePathSet = Directory.GetFiles(directoryPath, "*.dll", SearchOption.AllDirectories).ToList();

            return dllFilePathSet.Where(filePath => this.CheckAssembly(filePath, productName, minimumVersion)).SelectMany(filePath => this.LoadAssemby(filePath)).ToList();
        }

        public List<Type> LoadAssemby(string filePath)
        {
            Assembly assembly;

            assembly = Assembly.LoadFrom(filePath);
            OneDasConverter.AssemblySet.Add(assembly);

            return assembly.ExportedTypes.ToList();
        }

        public IEnumerable<Type> GetPluginsByBaseClass<TBaseClass>()
        {
            return _pluginDictionary.Where(entry => typeof(TBaseClass).IsAssignableFrom(entry.Key)).SelectMany(entry => entry.Value).ToList();
        }

        public IEnumerable<Type> Get<TPluginSettings>()
        {
            HashSet<Type> pluginSet;

            if (_pluginDictionary.TryGetValue(typeof(TPluginSettings), out pluginSet))
            {
                return pluginSet;
            }
            else
            {
                return new HashSet<Type>();
            }
        }

        public Type Get<TPluginSettings>(string pluginId)
        {
            return this.Get<TPluginSettings>().ToList().FirstOrDefault(x => x.GetFirstAttribute<PluginIdentificationAttribute>().Id == pluginId);
        }

        public void Add<T>(Type pluginType)
        {
            this.Add(typeof(T), pluginType);
        }

        public void Add(Type baseType, Type pluginType)
        {
            this.AddRange(baseType, new List<Type> { pluginType });
        }

        public void AddRange<T>(IEnumerable<Type> pluginTypeSet)
        {
            this.AddRange(typeof(T), pluginTypeSet);
        }

        public void AddRange(Type baseType, IEnumerable<Type> pluginTypeSet)
        {
            if (_pluginDictionary.ContainsKey(baseType))
            {
                pluginTypeSet.ToList().ForEach(type => _pluginDictionary[baseType].Add(type));
            }
            else
            {
                _pluginDictionary[baseType] = new HashSet<Type>(pluginTypeSet);
            }
        }

        public void Clear<T>()
        {
            if (_pluginDictionary.ContainsKey(typeof(T)))
            {
                _pluginDictionary.Clear();
            }
        }

        private void InternalScanAssembly(List<Type> typeSet)
        {
            List<Type> pluginConcreteTypeSet;
            HashSet<Type> pluginBaseTypeSet;

            pluginConcreteTypeSet = typeSet.Where(type => type.IsClass && !type.IsAbstract && (type.IsSubclassOf(typeof(PluginSettingsBase)) || type.IsSubclassOf(typeof(PluginLogicBase)))).ToList();
            pluginBaseTypeSet = new HashSet<Type>(pluginConcreteTypeSet.Select(type => type.BaseType));

            pluginBaseTypeSet.ToList().ForEach(baseType =>
            {
                this.AddRange(baseType, pluginConcreteTypeSet.Where(pluginType => pluginType.BaseType == baseType));
            });
        }

        private bool CheckAssembly(string filePath, string productName, Version minimumVersion)
        {
            Version productVersion;
            FileVersionInfo fileVersionInfo;

            fileVersionInfo = FileVersionInfo.GetVersionInfo(filePath);

            if (!string.IsNullOrEmpty(fileVersionInfo.FileVersion))
            {
                productVersion = new Version(fileVersionInfo.FileVersion);

                if (fileVersionInfo.ProductName == productName && productVersion.CompareTo(minimumVersion) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region "Factory"

        public IPluginSupporter BuildSupporter(Type pluginSettingsType)
        {
            Type pluginSupporterType;

            if (pluginSettingsType == null)
            {
                throw new ArgumentNullException();
            }

            pluginSupporterType = pluginSettingsType.GetFirstAttribute<PluginSupporterAttribute>()?.Type;

            if (pluginSupporterType == null)
            {
                throw new OneDasException(ErrorMessage.PluginProvider_NoPluginSupporterAttributeFound);
            }

            if (!typeof(IPluginSupporter).IsAssignableFrom(pluginSupporterType))
            {
                throw new OneDasException(ErrorMessage.PluginProvider_TypeDoesNotImplementIPluginSupporter);
            }

            return (IPluginSupporter)ActivatorUtilities.CreateInstance(_serviceProvider, pluginSupporterType);
        }

        public IPluginSupporter TryBuildSupporter(Type pluginSettingsType)
        {
            Type pluginSupporterType;

            if (pluginSettingsType == null)
            {
                throw new ArgumentNullException();
            }

            pluginSupporterType = pluginSettingsType.GetFirstAttribute<PluginSupporterAttribute>()?.Type;

            if (pluginSupporterType == null)
            {
                return null;
            }

            if (!typeof(IPluginSupporter).IsAssignableFrom(pluginSupporterType))
            {
                return null;
            }

            return (IPluginSupporter)ActivatorUtilities.CreateInstance(_serviceProvider, pluginSupporterType);
        }

        public TPluginLogic BuildContainer<TPluginLogic>(PluginSettingsBase pluginSettings, params object[] args) where TPluginLogic : PluginLogicBase
        {
            Type pluginLogicType;
            object[] argsExtended;

            argsExtended = args.Concat(new object[] { pluginSettings }).ToArray();
            pluginLogicType = pluginSettings.GetType().GetFirstAttribute<PluginContextAttribute>()?.PluginLogic;

            if (pluginLogicType == null)
            {
                throw new OneDasException(ErrorMessage.PluginProvider_NoMatchingTPluginLogicFound);
            }

            return (TPluginLogic)ActivatorUtilities.CreateInstance(_serviceProvider, pluginLogicType, argsExtended);
        }

        #endregion

        #region "Helper"

        public ActionResponse HandleActionRequest(ActionRequest actionRequest)
        {
            List<Type> typeSet;
            Type pluginSettingsType;
            ActionResponse actionResponse;
            IPluginSupporter pluginSupporter;

            typeSet = this.GetPluginsByBaseClass<PluginSettingsBase>().ToList();
            actionRequest.Validate();

            pluginSettingsType = typeSet.FirstOrDefault(x => x.GetFirstAttribute<PluginIdentificationAttribute>().Id == actionRequest.PluginId);

            if (pluginSettingsType != null)
            {
                pluginSupporter = this.BuildSupporter(pluginSettingsType);
                actionResponse = pluginSupporter.HandleActionRequest(actionRequest);
            }
            else
            {
                throw new Exception(ErrorMessage.PluginProvider_PluginNotFound);
            }

            return actionResponse;
        }

        public string GetStringResource(string pluginId, string resourceName)
        {
            List<Type> typeSet;
            Type type;
            Assembly assembly;

            typeSet = this.GetPluginsByBaseClass<PluginSettingsBase>().ToList();
            type = typeSet.FirstOrDefault(x => x.GetFirstAttribute<PluginIdentificationAttribute>().Id == pluginId);
            assembly = type?.Assembly;
            resourceName = $"{ assembly.GetName().Name }.{ resourceName }";

            if (assembly != null)
            {
                using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (resourceStream != null)
                    {
                        using (StreamReader reader = new StreamReader(resourceStream))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                    else
                    {
                        throw new OneDasException($"The requested resource of plugin ID = '{ pluginId }' and name = '{ resourceName }' could not be found.");
                    }
                }
            }
            else
            {
                throw new OneDasException($"The requested plugin with ID = '{ pluginId }' could not be found.");
            }
        }

        #endregion
    }
}
