using Microsoft.Extensions.DependencyInjection;
using OneDas.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OneDas.Plugin
{
    public class PluginManager
    {
        private IServiceProvider _serviceProvider;
        private Dictionary<Type, IEnumerable<Type>> _pluginDictionary;
        private static HashSet<Assembly> _assemblySet;

        static PluginManager()
        {
            _assemblySet = new HashSet<Assembly>();
        }

        public PluginManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _pluginDictionary = new Dictionary<Type, IEnumerable<Type>>();
        }

        #region "Hive"

        public List<Type> LoadAssemblies(string directoryPath, string productName, Version minimumVersion)
        {
            List<string> dllFilePathSet;

            dllFilePathSet = Directory.GetFiles(directoryPath, "*.dll", SearchOption.AllDirectories).ToList();

            return dllFilePathSet.SelectMany(dllFilePath =>
            {
                Version productVersion;
                Assembly assembly;
                FileVersionInfo fileVersionInfo;
                IEnumerable<PluginInitializerAttribute> attributeSet;

                assembly = default;

                try
                {
                    fileVersionInfo = FileVersionInfo.GetVersionInfo(dllFilePath);

                    if (!string.IsNullOrEmpty(fileVersionInfo.FileVersion))
                    {
                        productVersion = new Version(fileVersionInfo.FileVersion);

                        if (fileVersionInfo.ProductName == productName && productVersion.CompareTo(minimumVersion) >= 0)
                        {
                            assembly = Assembly.Load(AssemblyName.GetAssemblyName(dllFilePath));
                        }
                    }
                }
                catch
                {
                    return new List<Type>();
                }

                if (assembly != null)
                {
                    if (!_assemblySet.Contains(assembly))
                    {
                        _assemblySet.Add(assembly);

                        assembly.ExportedTypes.ToList().ForEach(type =>
                        {
                            attributeSet = type.GetCustomAttributes(typeof(PluginInitializerAttribute), false).Cast<PluginInitializerAttribute>();
                            attributeSet.ToList().ForEach(x => x.InitializerAction(_serviceProvider));
                        });
                    }

                    return assembly.ExportedTypes;
                }
                else
                {
                    return new List<Type>();
                }
            }).ToList();
        }

        public IEnumerable<Type> GetPluginsByBaseClass<TBaseClass>()
        {
            return _pluginDictionary.Where(entry => typeof(TBaseClass).IsAssignableFrom(entry.Key)).SelectMany(entry => entry.Value).ToList();
        }

        public IEnumerable<Type> Get<T>()
        {
            IEnumerable<Type> pluginSet;

            if (_pluginDictionary.TryGetValue(typeof(T), out pluginSet))
            {
                return pluginSet;
            }
            else
            {
                return new Type[] { };
            }
        }

        public void Add<T>(Type plugin)
        {
            this.AddRange<T>(new List<Type> { plugin });
        }

        public void AddRange<T>(IEnumerable<Type> pluginSet)
        {
            if (_pluginDictionary.ContainsKey(typeof(T)))
            {
                List<Type> newTypeSet;

                newTypeSet = _pluginDictionary[typeof(T)].ToList();
                newTypeSet.AddRange(pluginSet);

                _pluginDictionary[typeof(T)] = newTypeSet;
            }
            else
            {
                _pluginDictionary[typeof(T)] = pluginSet;
            }
        }

        public void Clear<T>()
        {
            if (_pluginDictionary.ContainsKey(typeof(T)))
            {
                _pluginDictionary.Clear();
            }
        }

        public static IEnumerable<Assembly> AssemblySet
        {
            get
            {
                return _assemblySet;
            }
        }

        #endregion

        #region "Factory"

        public TPluginSettings BuildSettings<TPluginSettings>(Type pluginSettingsType, params object[] args) where TPluginSettings : PluginSettingsBase
        {
            if (pluginSettingsType == null)
            {
                throw new ArgumentNullException();
            }

            if (!typeof(TPluginSettings).IsAssignableFrom(pluginSettingsType))
            {
                throw new Exception(ErrorMessage.PluginManager_TypeNotInheritedFromTPluginSettings);
            }

            return (TPluginSettings)ActivatorUtilities.CreateInstance(_serviceProvider, pluginSettingsType, args);
        }

        public TPluginSettings BuildSettings<TPluginSettings>(string pluginName, params object[] args) where TPluginSettings : PluginSettingsBase
        {
            IEnumerable<Type> pluginSettingsTypeSet;
            Type pluginSettingsType;

            pluginSettingsTypeSet = this.Get<TPluginSettings>();
            pluginSettingsType = pluginSettingsTypeSet.ToList().FirstOrDefault(x => x.GetFirstAttribute<PluginIdentificationAttribute>().Name == pluginName);

            if (pluginSettingsType == null)
            {
                throw new Exception(ErrorMessage.PluginManager_NoMatchingTPluginSettingsFound);
            }

            return (TPluginSettings)ActivatorUtilities.CreateInstance(_serviceProvider, pluginSettingsType, args);
        }

        public TPluginLogic BuildSettingsContainer<TPluginLogic>(PluginSettingsBase pluginSettings, params object[] args) where TPluginLogic : PluginLogicBase
        {
            Type pluginLogicType;

            pluginLogicType = pluginSettings.GetType().GetFirstAttribute<PluginContextAttribute>().PluginLogic;

            if (pluginLogicType == null)
            {
                throw new Exception(ErrorMessage.PluginManager_NoMatchingTPluginLogicFound);
            }

            return (TPluginLogic)ActivatorUtilities.CreateInstance(_serviceProvider, pluginLogicType, pluginSettings, args);
        }

        #endregion
    }
}