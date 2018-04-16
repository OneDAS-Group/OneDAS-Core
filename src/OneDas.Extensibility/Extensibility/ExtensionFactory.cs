using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OneDas.Extensibility
{
    public class ExtensionFactory : IExtensionFactory
    {
        #region "Fields"

        private IServiceProvider _serviceProvider;
        private HashSet<Type> _pluginSet;

        #endregion

        #region "Constructors"

        public ExtensionFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _pluginSet = new HashSet<Type>();
        }

        #endregion

        #region "Hive"      

        public void ScanAssembly(Assembly assembly)
        {
            List<Type> foundTypeSet;

            foundTypeSet = assembly.ExportedTypes.Where(type => type.IsClass && !type.IsAbstract && (type.IsSubclassOf(typeof(ExtensionSettingsBase)) || type.IsSubclassOf(typeof(ExtensionLogicBase)))).ToList();

            this.AddRange(foundTypeSet);
        }

        public Type GetSettings(string id)
        {
            return _pluginSet.FirstOrDefault(type => type.GetFirstAttribute<ExtensionIdentificationAttribute>()?.Id == id);
        }

        public Type GetLogic(string id)
        {
            return this.GetSettings(id)?.GetFirstAttribute<ExtensionContextAttribute>().LogicType;
        }

        public IEnumerable<Type> Get<TPluginBase>()
        {
            return _pluginSet.Where(type => type.IsSubclassOf(typeof(TPluginBase))).ToList();
        }

        public IEnumerable<ExtensionIdentificationAttribute> GetIdentifications<TPluginSettings>() where TPluginSettings : ExtensionSettingsBase
        {
            return this.Get<TPluginSettings>().Select(settingsType =>
            {
                ExtensionIdentificationAttribute attribute;

                attribute = settingsType.GetFirstAttribute<ExtensionIdentificationAttribute>();
                attribute.ProductVersion = FileVersionInfo.GetVersionInfo(settingsType.Assembly.Location).ProductVersion;

                return attribute;
            }).ToList();
        }

        public void Add(Type type)
        {
            this.AddRange(new List<Type> { type });
        }

        public void AddRange(IEnumerable<Type> typeSet)
        {
            typeSet.ToList().ForEach(pluginType =>
            {
                // create IPluginSupporter and call Initialize once
                if (!_pluginSet.Contains(pluginType))
                {
                    this.TryBuildSupporter(pluginType)?.Initialize();
                }

                _pluginSet.Add(pluginType);
            });
        }

        public void Clear()
        {
            _pluginSet.Clear();
        }

        #endregion

        #region "Factory"

        public IExtensionSupporter BuildSupporter(Type settingsType)
        {
            Type supporterType;

            if (settingsType == null)
            {
                throw new ArgumentNullException();
            }

            if (!settingsType.IsSubclassOf(typeof(ExtensionSettingsBase)))
            {
                throw new Exception(ErrorMessage.ExtensionFactory_TypeDoesNotInheritFromPluginSettingsBase);
            }

            supporterType = settingsType.GetFirstAttribute<ExtensionSupporterAttribute>()?.Type;

            if (supporterType == null)
            {
                throw new Exception(ErrorMessage.ExtensionFactory_NoPluginSupporterAttributeFound);
            }

            if (!typeof(IExtensionSupporter).IsAssignableFrom(supporterType))
            {
                throw new Exception(ErrorMessage.ExtensionFactory_TypeDoesNotImplementIPluginSupporter);
            }

            return (IExtensionSupporter)ActivatorUtilities.CreateInstance(_serviceProvider, supporterType);
        }

        public IExtensionSupporter TryBuildSupporter(Type settingsType)
        {
            Type pluginSupporterType;

            if (settingsType == null)
            {
                throw new ArgumentNullException();
            }

            if (!settingsType.IsSubclassOf(typeof(ExtensionSettingsBase)))
            {
                return null;
            }

            pluginSupporterType = settingsType.GetFirstAttribute<ExtensionSupporterAttribute>()?.Type;

            if (pluginSupporterType == null)
            {
                return null;
            }

            if (!typeof(IExtensionSupporter).IsAssignableFrom(pluginSupporterType))
            {
                return null;
            }

            return (IExtensionSupporter)ActivatorUtilities.CreateInstance(_serviceProvider, pluginSupporterType);
        }

        public TPluginLogic BuildLogic<TPluginLogic>(ExtensionSettingsBase settings, params object[] args) where TPluginLogic : ExtensionLogicBase
        {
            Type logicType;
            object[] argsExtended;

            argsExtended = args.Concat(new object[] { settings }).ToArray();
            logicType = settings.GetType().GetFirstAttribute<ExtensionContextAttribute>()?.LogicType;

            if (logicType == null)
            {
                throw new Exception(ErrorMessage.ExtensionFactory_NoMatchingTPluginLogicFound);
            }

            return (TPluginLogic)ActivatorUtilities.CreateInstance(_serviceProvider, logicType, argsExtended);
        }

        #endregion

        #region "Helper"

        public ActionResponse HandleActionRequest(ActionRequest actionRequest)
        {
            List<Type> typeSet;
            Type pluginSettingsType;
            ActionResponse actionResponse;
            IExtensionSupporter pluginSupporter;

            typeSet = this.Get<ExtensionSettingsBase>().ToList();
            actionRequest.Validate();

            pluginSettingsType = typeSet.FirstOrDefault(x => x.GetFirstAttribute<ExtensionIdentificationAttribute>().Id == actionRequest.PluginId);

            if (pluginSettingsType != null)
            {
                pluginSupporter = this.BuildSupporter(pluginSettingsType);
                actionResponse = pluginSupporter.HandleActionRequest(actionRequest);
            }
            else
            {
                throw new Exception(ErrorMessage.ExtensionFactory_PluginNotFound);
            }

            return actionResponse;
        }

        public string GetStringResource(string id, string resourceName)
        {
            List<Type> typeSet;
            Type type;
            Assembly assembly;

            typeSet = this.Get<ExtensionSettingsBase>().ToList();
            type = typeSet.FirstOrDefault(x => x.GetFirstAttribute<ExtensionIdentificationAttribute>().Id == id);
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
                        throw new Exception($"The requested resource of plugin ID = '{ id }' and name = '{ resourceName }' could not be found.");
                    }
                }
            }
            else
            {
                throw new Exception($"The requested plugin with ID = '{ id }' could not be found.");
            }
        }

        #endregion
    }
}
