using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using OneDas.Infrastructure;
using OneDas.Plugin;

namespace OneDas.Engine.Serialization
{
    public class OneDasConverter : JsonConverter
    {
        public static HashSet<Assembly> AssemblySet;

        static OneDasConverter()
        {
            OneDasConverter.AssemblySet = new HashSet<Assembly>();

            OneDasConverter.AssemblySet.Add(Assembly.GetExecutingAssembly());
            OneDasConverter.AssemblySet.Add(typeof(OneDasProject).Assembly);

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName.Contains("OneDAS"))
                {
                    OneDasConverter.AssemblySet.Add(assembly);
                }
            } 
        }

        public override bool CanConvert(Type objectType)
        {
            return !objectType.IsPrimitive && !objectType.IsArray && OneDasConverter.AssemblySet.Contains(objectType.Assembly);
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
            else if (value is OneDasModule) // here also
            {
                SerializationHelper.JsonSerializer.Serialize(writer, value, typeof(OneDasModule));
            }
            else
            {
                SerializationHelper.JsonSerializer.Serialize(writer, value, typeof(JsonSerializer));
            }
        }
    }
}
