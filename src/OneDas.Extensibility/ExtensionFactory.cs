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
        private HashSet<Type> _extensionSet;

        #endregion

        #region "Constructors"

        public ExtensionFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _extensionSet = new HashSet<Type>();
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
            return _extensionSet.FirstOrDefault(type => type.GetFirstAttribute<ExtensionIdentificationAttribute>()?.Id == id);
        }

        public Type GetLogic(string id)
        {
            return this.GetSettings(id)?.GetFirstAttribute<ExtensionContextAttribute>().LogicType;
        }

        public IEnumerable<Type> Get<TExtensionBase>()
        {
            return _extensionSet.Where(type => type.IsSubclassOf(typeof(TExtensionBase))).ToList();
        }

        public IEnumerable<ExtensionIdentificationAttribute> GetIdentifications<TExtensionSettings>() where TExtensionSettings : ExtensionSettingsBase
        {
            return this.Get<TExtensionSettings>().Select(settingsType =>
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
            typeSet.ToList().ForEach(type =>
            {
                // create IExtensionSupporter and call Initialize once
                if (!_extensionSet.Contains(type))
                {
                    this.TryBuildSupporter(type)?.Initialize();
                }

                _extensionSet.Add(type);
            });
        }

        public void Clear()
        {
            _extensionSet.Clear();
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
                throw new Exception(ErrorMessage.ExtensionFactory_TypeDoesNotInheritFromExtensionSettingsBase);
            }

            supporterType = settingsType.GetFirstAttribute<ExtensionSupporterAttribute>()?.Type;

            if (supporterType == null)
            {
                throw new Exception(ErrorMessage.ExtensionFactory_NoExtensionSupporterAttributeFound);
            }

            if (!typeof(IExtensionSupporter).IsAssignableFrom(supporterType))
            {
                throw new Exception(ErrorMessage.ExtensionFactory_TypeDoesNotImplementIExtensionSupporter);
            }

            return (IExtensionSupporter)ActivatorUtilities.CreateInstance(_serviceProvider, supporterType);
        }

        public IExtensionSupporter TryBuildSupporter(Type settingsType)
        {
            Type supporterType;

            if (settingsType == null)
            {
                throw new ArgumentNullException();
            }

            if (!settingsType.IsSubclassOf(typeof(ExtensionSettingsBase)))
            {
                return null;
            }

            supporterType = settingsType.GetFirstAttribute<ExtensionSupporterAttribute>()?.Type;

            if (supporterType == null)
            {
                return null;
            }

            if (!typeof(IExtensionSupporter).IsAssignableFrom(supporterType))
            {
                return null;
            }

            return (IExtensionSupporter)ActivatorUtilities.CreateInstance(_serviceProvider, supporterType);
        }

        public TExtensionLogic BuildLogic<TExtensionLogic>(ExtensionSettingsBase settings, params object[] args) where TExtensionLogic : ExtensionLogicBase
        {
            Type logicType;
            object[] argsExtended;

            argsExtended = args.Concat(new object[] { settings }).ToArray();
            logicType = settings.GetType().GetFirstAttribute<ExtensionContextAttribute>()?.LogicType;

            if (logicType == null)
            {
                throw new Exception(ErrorMessage.ExtensionFactory_NoMatchingTExtensionLogicFound);
            }

            return (TExtensionLogic)ActivatorUtilities.CreateInstance(_serviceProvider, logicType, argsExtended);
        }

        #endregion

        #region "Helper"

        public ActionResponse HandleActionRequest(ActionRequest actionRequest)
        {
            List<Type> typeSet;
            Type settingsType;
            ActionResponse actionResponse;
            IExtensionSupporter supporter;

            typeSet = this.Get<ExtensionSettingsBase>().ToList();
            actionRequest.Validate();

            settingsType = typeSet.FirstOrDefault(x => x.GetFirstAttribute<ExtensionIdentificationAttribute>().Id == actionRequest.ExtensionId);

            if (settingsType != null)
            {
                supporter = this.BuildSupporter(settingsType);
                actionResponse = supporter.HandleActionRequest(actionRequest);
            }
            else
            {
                throw new Exception(ErrorMessage.ExtensionFactory_ExtensionNotFound);
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
            resourceName = $"{ assembly.GetName().Name }.{ resourceName }"; // actually it should be the root namespace instead of assembly name

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
                        throw new Exception($"The requested resource of extension ID = '{ id }' and name = '{ resourceName }' could not be found.");
                    }
                }
            }
            else
            {
                throw new Exception($"The requested extension with ID = '{ id }' could not be found.");
            }
        }

        #endregion
    }
}
