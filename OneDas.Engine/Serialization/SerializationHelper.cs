using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OneDas.Infrastructure;
using System;
using System.IO;

namespace OneDas.Engine.Serialization
{
    internal static class SerializationHelper
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
    }
}