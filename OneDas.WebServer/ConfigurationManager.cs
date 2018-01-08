using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OneDas.Engine.Serialization;
using System;
using System.IO;

namespace OneDas.WebServer
{
    public static class ConfigurationManager<T>
    {
        private static string _configurationDirectoryPath;
        private static string _configurationFileName;

        public static T Settings { get; private set; }

        public static void Initialize(string configurationDirectoryPath, string configurationFileName)
        {
            ConfigurationBuilder configurationBuilder;

            _configurationDirectoryPath = configurationDirectoryPath;
            _configurationFileName = configurationFileName;

            configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.SetBasePath(configurationDirectoryPath);
            configurationBuilder.AddJsonFile(configurationFileName);

            if (!File.Exists(Path.Combine(configurationDirectoryPath, configurationFileName)))
            {
                using (StreamWriter streamWriter = new StreamWriter(Path.Combine(configurationDirectoryPath, configurationFileName)))
                {
                    SerializationHelper.JsonSerializer.Serialize(streamWriter, Activator.CreateInstance<T>());
                }
            }

            ConfigurationManager<T>.Settings = configurationBuilder.Build().Get<T>();
        }

        public static void Save()
        {
            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(_configurationDirectoryPath, _configurationFileName)))
            {
                string rawJson;

                rawJson = JsonConvert.SerializeObject(ConfigurationManager<T>.Settings, Formatting.Indented, SerializationHelper.CreateDefaultSerializationSettings());
                new JsonTextWriter(streamWriter).WriteRaw(rawJson);
            }
        }
    }
}
