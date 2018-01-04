using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OneDas.Infrastructure;

namespace OneDas.Engine.Serialization
{
    public delegate object DeserializationDelegate(object jObject, Type type);

    public static class SerializationHelper
    {
        private static JsonSerializer _jsonSerializer;

        public static JsonSerializer JsonSerializer
        {
            get
            {
                if (_jsonSerializer == null)
                {
                    _jsonSerializer = JsonSerializer.Create(SerializationHelper.CreateDefaultSerializationSettings());
                }

                return _jsonSerializer;
            }
            set
            {
                _jsonSerializer = value;
            }
        }

        public static object Deserialize(object input, Type type)
        {
            return ((JObject)input).ToObject(type, SerializationHelper.JsonSerializer);
        }

        public static JsonSerializerSettings CreateDefaultSerializationSettings()
        {
            return new JsonSerializerSettings()
            {
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = new KnownTypesBinder(),
                ContractResolver = new ConstructorlessContractResolver()
            };
        }

        public static Type GetType(string typeName)
        {
            Type type = Type.GetType(typeName);

            if (type != null)
            {
                return type;
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(typeName);

                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }

        internal static ProjectDescription GetProjectDescriptonFromFile(string filePath)
        {
            return JObject.Parse(File.ReadAllText(filePath))["Description"].ToObject<ProjectDescription>();
        }
    }
}