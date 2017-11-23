using System;
using Newtonsoft.Json.Serialization;

namespace OneDas.Main.Serialization
{
    public class KnownTypesBinder : ISerializationBinder
    {
        public Type BindToType(string assemblyName, string typeName)
        {
            return SerializationHelper.GetType(typeName);
        }

        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = serializedType.Assembly.GetName().Name;
            typeName = serializedType.FullName;
        }
    }
}
