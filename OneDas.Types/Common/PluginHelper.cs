using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace OneDas.Common
{
    public static class PluginHelper<T>
    {
        public static IEnumerable<Type> GetTypeSet(string directoryPath, Version minimumVersion)
        {
            Version productVersion;

            IEnumerable<string> dllFilePathSet;
            ICollection<Type> pluginSet;

            dllFilePathSet = Directory.GetFiles(directoryPath, "*.dll", SearchOption.AllDirectories);
            pluginSet = new List<Type>();

            foreach (string dllFilePath in dllFilePathSet)
            {
                Assembly assembly = default(Assembly);

                try
                {
                    productVersion = new Version(FileVersionInfo.GetVersionInfo(dllFilePath).FileVersion);

                    if (productVersion.CompareTo(minimumVersion) > 0)
                    {
                        assembly = Assembly.Load(AssemblyName.GetAssemblyName(dllFilePath));
                    }
                }
                catch (Exception)
                {
                    continue;
                }

                if (assembly != null)
                {
                    foreach (Type type in assembly.ExportedTypes)
                    {
                        try
                        {
                            if (type.IsInterface | type.IsAbstract)
                            {
                                continue;
                            }
                            else
                            {
                                if (type.GetInterface(typeof(T).FullName) != null || type.IsSubclassOf(typeof(T)))
                                {
                                    pluginSet.Add(type);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                }
            }

            return pluginSet;
        }
    }
}
