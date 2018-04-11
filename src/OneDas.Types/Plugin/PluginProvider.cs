using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OneDas.Plugin
{
    public class PluginProvider : IPluginProvider
    {
        #region "Fields"

        private IServiceProvider _serviceProvider;
        private HashSet<Type> _pluginSet;

        #endregion

        #region "Constructors"

        public PluginProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _pluginSet = new HashSet<Type>();
        }

        #endregion

        #region "Hive"      

        public void ScanAssembly(Assembly assembly)
        {
            List<Type> foundTypeSet;

            foundTypeSet = assembly.ExportedTypes.Where(type => type.IsClass && !type.IsAbstract && (type.IsSubclassOf(typeof(PluginSettingsBase)) || type.IsSubclassOf(typeof(PluginLogicBase)))).ToList();

            this.AddRange(foundTypeSet);
        }

        public Type GetSettings(string pluginId)
        {
            return _pluginSet.FirstOrDefault(type => type.GetFirstAttribute<PluginIdentificationAttribute>()?.Id == pluginId);
        }

        public Type GetLogic(string pluginId)
        {
            return this.GetSettings(pluginId)?.GetFirstAttribute<PluginContextAttribute>().PluginLogic;
        }

        public IEnumerable<Type> Get<TPluginBase>()
        {
            return _pluginSet.Where(type => type.IsSubclassOf(typeof(TPluginBase))).ToList();
        }

        public IEnumerable<PluginIdentificationAttribute> GetIdentifications<TPluginSettings>() where TPluginSettings : PluginSettingsBase
        {
            return this.Get<TPluginSettings>().Select(settingsType =>
            {
                PluginIdentificationAttribute attribute;

                attribute = settingsType.GetFirstAttribute<PluginIdentificationAttribute>();
                attribute.ProductVersion = FileVersionInfo.GetVersionInfo(settingsType.Assembly.Location).ProductVersion;

                return attribute;
            }).ToList();
        }

        public void Add(Type pluginType)
        {
            this.AddRange(new List<Type> { pluginType });
        }

        public void AddRange(IEnumerable<Type> pluginTypeSet)
        {
            pluginTypeSet.ToList().ForEach(pluginType =>
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

        public IPluginSupporter BuildSupporter(Type pluginSettingsType)
        {
            Type pluginSupporterType;

            if (pluginSettingsType == null)
            {
                throw new ArgumentNullException();
            }

            if (!pluginSettingsType.IsSubclassOf(typeof(PluginSettingsBase)))
            {
                throw new Exception(ErrorMessage.PluginProvider_TypeDoesNotInheritFromPluginSettingsBase);
            }

            pluginSupporterType = pluginSettingsType.GetFirstAttribute<PluginSupporterAttribute>()?.Type;

            if (pluginSupporterType == null)
            {
                throw new Exception(ErrorMessage.PluginProvider_NoPluginSupporterAttributeFound);
            }

            if (!typeof(IPluginSupporter).IsAssignableFrom(pluginSupporterType))
            {
                throw new Exception(ErrorMessage.PluginProvider_TypeDoesNotImplementIPluginSupporter);
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

            if (!pluginSettingsType.IsSubclassOf(typeof(PluginSettingsBase)))
            {
                return null;
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

        public TPluginLogic BuildLogic<TPluginLogic>(PluginSettingsBase pluginSettings, params object[] args) where TPluginLogic : PluginLogicBase
        {
            Type pluginLogicType;
            object[] argsExtended;

            argsExtended = args.Concat(new object[] { pluginSettings }).ToArray();
            pluginLogicType = pluginSettings.GetType().GetFirstAttribute<PluginContextAttribute>()?.PluginLogic;

            if (pluginLogicType == null)
            {
                throw new Exception(ErrorMessage.PluginProvider_NoMatchingTPluginLogicFound);
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

            typeSet = this.Get<PluginSettingsBase>().ToList();
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

            typeSet = this.Get<PluginSettingsBase>().ToList();
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
                        throw new Exception($"The requested resource of plugin ID = '{ pluginId }' and name = '{ resourceName }' could not be found.");
                    }
                }
            }
            else
            {
                throw new Exception($"The requested plugin with ID = '{ pluginId }' could not be found.");
            }
        }

        #endregion
    }
}
