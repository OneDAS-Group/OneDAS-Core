using System.Text.Json.Serialization;

namespace OneDas.DataManagement.Explorer.Core
{
    public record UserCredentials
    {
        /// <example>test@onedas.org</example>
        [JsonPropertyName("username")]
        public string Username { get; set; }

        /// <example>#test0/User1</example>
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}
