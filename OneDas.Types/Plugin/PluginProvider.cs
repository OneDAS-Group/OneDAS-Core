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
    public class PluginProvider
    {
        private IServiceProvider _serviceProvider;
        private Dictionary<Type, IEnumerable<Type>> _pluginDictionary;
        private static HashSet<Assembly> _assemblySet;

        static PluginProvider()
        {
            _assemblySet = new HashSet<Assembly>();
        }

        public PluginProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _pluginDictionary = new Dictionary<Type, IEnumerable<Type>>();
        }

        #region "Hive"

        public void ScanAssembly<TPluginLogicBase, TPluginSettinsBase>(string filePath, string productName, Version minimumVersion)
        {
            List<Type> pluginTypeSet;

            pluginTypeSet = this.LoadAssemby(filePath, productName, minimumVersion);

            this.AddRange<TPluginLogicBase>(pluginTypeSet.Where(pluginType => typeof(TPluginLogicBase).IsAssignableFrom(pluginType)).ToList());
            this.AddRange<TPluginSettinsBase>(pluginTypeSet.Where(pluginType => typeof(TPluginSettinsBase).IsAssignableFrom(pluginType)).ToList());
        }

        public void ScanAssemblies<TPluginLogicBase, TPluginSettinsBase>(string directoryPath, string productName, Version minimumVersion)
        {
            List<Type> pluginTypeSet;

            pluginTypeSet = this.LoadAssemblies(directoryPath, productName, minimumVersion);

            this.AddRange<TPluginLogicBase>(pluginTypeSet.Where(pluginType => typeof(TPluginLogicBase).IsAssignableFrom(pluginType)).ToList());
            this.AddRange<TPluginSettinsBase>(pluginTypeSet.Where(pluginType => typeof(TPluginSettinsBase).IsAssignableFrom(pluginType)).ToList());
        }

        public List<Type> LoadAssemblies(string directoryPath, string productName, Version minimumVersion)
        {
            List<string> dllFilePathSet;

            dllFilePathSet = Directory.GetFiles(directoryPath, "*.dll", SearchOption.AllDirectories).ToList();

            return dllFilePathSet.SelectMany(filePath => this.LoadAssemby(filePath, productName, minimumVersion)).ToList();
        }

        public List<Type> LoadAssemby(string filePath, string productName, Version minimumVersion)
        {
            Version productVersion;
            Assembly assembly;
            FileVersionInfo fileVersionInfo;

            assembly = default;

            try
            {
                fileVersionInfo = FileVersionInfo.GetVersionInfo(filePath);

                if (!string.IsNullOrEmpty(fileVersionInfo.FileVersion))
                {
                    productVersion = new Version(fileVersionInfo.FileVersion);

                    if (fileVersionInfo.ProductName == productName && productVersion.CompareTo(minimumVersion) >= 0)
                    {
                        assembly = Assembly.Load(AssemblyName.GetAssemblyName(filePath));
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
                }

                return assembly.ExportedTypes.ToList();
            }
            else
            {
                return new List<Type>();
            }
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

        public void Add<T>(Type pluginType)
        {
            this.AddRange<T>(new List<Type> { pluginType });
        }

        public void AddRange<T>(IEnumerable<Type> pluginTypeSet)
        {
            if (_pluginDictionary.ContainsKey(typeof(T)))
            {
                List<Type> newTypeSet;

                newTypeSet = _pluginDictionary[typeof(T)].ToList();
                newTypeSet.AddRange(pluginTypeSet);

                _pluginDictionary[typeof(T)] = newTypeSet;
            }
            else
            {
                _pluginDictionary[typeof(T)] = pluginTypeSet;
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

        public IPluginSupporter BuildSupporter(Type pluginSettingsType, params object[] args)
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

            if (typeof(IPluginSupporter).IsAssignableFrom(pluginSupporterType))
            {
                throw new OneDasException(ErrorMessage.PluginProvider_TypeDoesNotImplementIPluginSupporter);
            }

            return (IPluginSupporter)ActivatorUtilities.CreateInstance(_serviceProvider, pluginSupporterType, args);
        }

        public TPluginSettings BuildSettings<TPluginSettings>(Type pluginSettingsType, params object[] args) where TPluginSettings : PluginSettingsBase
        {
            if (pluginSettingsType == null)
            {
                throw new ArgumentNullException();
            }

            if (!typeof(TPluginSettings).IsAssignableFrom(pluginSettingsType))
            {
                throw new OneDasException(ErrorMessage.PluginProvider_TypeNotInheritedFromTPluginSettings);
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
                throw new OneDasException(ErrorMessage.PluginProvider_NoMatchingTPluginSettingsFound);
            }

            return (TPluginSettings)ActivatorUtilities.CreateInstance(_serviceProvider, pluginSettingsType, args);
        }

        public TPluginLogic BuildSettingsContainer<TPluginLogic>(PluginSettingsBase pluginSettings, params object[] args) where TPluginLogic : PluginLogicBase
        {
            Type pluginLogicType;

            pluginLogicType = pluginSettings.GetType().GetFirstAttribute<PluginContextAttribute>().PluginLogic;

            if (pluginLogicType == null)
            {
                throw new OneDasException(ErrorMessage.PluginProvider_NoMatchingTPluginLogicFound);
            }

            return (TPluginLogic)ActivatorUtilities.CreateInstance(_serviceProvider, pluginLogicType, pluginSettings, args);
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