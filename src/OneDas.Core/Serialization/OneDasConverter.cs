using Newtonsoft.Json;
using OneDas.Core.ProjectManagement;
using OneDas.Extensibility;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace OneDas.Core.Serialization
{
    public class OneDasConverter : JsonConverter
    {
        public static HashSet<Assembly> AssemblySet;

        static OneDasConverter()
        {
            AssemblyProductAttribute assemblyProductAttribute;

            OneDasConverter.AssemblySet = new HashSet<Assembly>();

            OneDasConverter.AssemblySet.Add(Assembly.GetExecutingAssembly());
            OneDasConverter.AssemblySet.Add(typeof(OneDasProject).Assembly);

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                assemblyProductAttribute = assembly.GetCustomAttribute<AssemblyProductAttribute>();

                if (assemblyProductAttribute != null && assemblyProductAttribute.Product.Contains("OneDAS"))
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
            if (value is ExtensionSettingsBase) // if root object should have $type property
            {
                SerializationHelper.JsonSerializer.Serialize(writer, value, typeof(ExtensionSettingsBase));
            }
            else if (value is OneDasModule) // here also
            {
                SerializationHelper.JsonSerializer.Serialize(writer, value, typeof(OneDasModule));
            }
            else
            {
                SerializationHelper.JsonSerializer.Serialize(writer, value);
            }
        }
    }
}
