using System.IO;
using System.Text.Json;

namespace OneDas.DataManagement
{
    public static class JsonSerializerHelper
    {
        public static void Serialize<T>(T value, string filePath)
        {
            var options = new JsonSerializerOptions() { WriteIndented = true };
            var jsonString = JsonSerializer.Serialize(value, options);

            File.WriteAllText(filePath, jsonString);
        }

        public static T Deserialize<T>(string filePath)
        {
            var jsonString = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<T>(jsonString);
        }
    }
}
