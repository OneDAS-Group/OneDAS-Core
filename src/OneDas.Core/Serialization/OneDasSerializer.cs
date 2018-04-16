using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OneDas.Infrastructure;

namespace OneDas.Core.Serialization
{
    public class OneDasSerializer : IOneDasSerializer
    {
        private JsonSerializer _jsonSerializer;
        private JsonSerializerSettings _jsonSerializerSettings;

        public OneDasSerializer()
        {
            _jsonSerializerSettings = SerializationHelper.CreateDefaultSerializationSettings();
            _jsonSerializerSettings.Formatting = Formatting.Indented;

            _jsonSerializer = JsonSerializer.Create(_jsonSerializerSettings);
        }

        public string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value, _jsonSerializerSettings);
        }

        public T Deserialize<T>(object jObject)
        {
            return ((JObject)jObject).ToObject<T>(_jsonSerializer);
        }

        public T Deserialize<T>(string rawJson)
        {
            JObject jObject;

            jObject = JObject.Parse(rawJson);

            return jObject.ToObject<T>(_jsonSerializer);
        }
    }
}
