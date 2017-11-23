using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using OneDas.Infrastructure;
using OneDas.Plugin;

namespace OneDas.Main.Serialization
{
    public class OneDasConverter : JsonConverter
    {
        private static HashSet<Assembly> _assemblySet;

        static OneDasConverter()
        {
            _assemblySet = new HashSet<Assembly>(PluginHive.AssemblySet);
            _assemblySet.Add(Assembly.GetExecutingAssembly());
            _assemblySet.Add(typeof(Project).Assembly);

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName.Contains("OneDAS"))
                {
                    _assemblySet.Add(assembly);
                }
            } 
        }

        public override bool CanConvert(Type objectType)
        {
            return !objectType.IsPrimitive && !objectType.IsArray && _assemblySet.Contains(objectType.Assembly);
        }
 
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return SerializationHelper.JsonSerializer.Deserialize(reader, objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is PluginSettingsBase) // if root object should have $type property
            {
                SerializationHelper.JsonSerializer.Serialize(writer, value, typeof(PluginSettingsBase));
            }
            else
            {
                SerializationHelper.JsonSerializer.Serialize(writer, value, typeof(JsonSerializer));
            }
        }
    }
}
