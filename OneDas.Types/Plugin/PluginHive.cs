using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using OneDas.Common;

namespace OneDas.Plugin
{
    public static class PluginHive
    {
        private static Dictionary<Type, IEnumerable<Type>> _pluginDictionary;
        private static HashSet<Assembly> _assemblySet;

        static PluginHive()
        {
            _pluginDictionary = new Dictionary<Type, IEnumerable<Type>>();
            _assemblySet = new HashSet<Assembly>();
        }

        public static void LoadPluginSet<T>(string directoryPath)
        {
            Version minimumVersion;

            IEnumerable<Type> typeSet;
            IEnumerable<PluginInitializerAttribute> attributeSet;

            minimumVersion = new Version(new Version(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion).Major, 0, 0, 0);
            typeSet = PluginHelper<T>.GetTypeSet(directoryPath, minimumVersion).ToList();

            typeSet.ToList().ForEach(type =>
            {
                attributeSet = type.GetCustomAttributes(typeof(PluginInitializerAttribute), false).Cast<PluginInitializerAttribute>();
                attributeSet.ToList().ForEach(x => x.InitializerAction());
            });

            PluginHive.AddPluginSet<T>(typeSet);

            typeSet.ToList().ForEach(type => _assemblySet.Add(type.Assembly));

            //var aggregateCatalog = new AggregateCatalog();

            //foreach (var path in Directory.EnumerateDirectories(directoryPath, "*", SearchOption.TopDirectoryOnly))
            //{
            //    aggregateCatalog.Catalogs.Add(new DirectoryCatalog(path));
            //}

            //var container = new CompositionContainer(aggregateCatalog);
        }

        public static IEnumerable<Type> GetPluginCategorySet<TBaseClass>()
        {
            return _pluginDictionary.Where(entry => typeof(TBaseClass).IsAssignableFrom(entry.Key)).SelectMany(entry => entry.Value).ToList();
        }

        public static IEnumerable<Type> GetPluginSet<T>()
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

        public static void AddPlugin<T>(Type plugin)
        {
            PluginHive.AddPluginSet<T>(new List<Type> { plugin });
        }

        public static void AddPluginSet<T>(IEnumerable<Type> pluginSet)
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

        public static void ClearPluginSet<T>()
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
    }
}
