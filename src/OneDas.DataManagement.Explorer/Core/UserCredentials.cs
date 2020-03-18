using System.Text.Json.Serialization;

namespace OneDas.DataManagement.Explorer.Core
{
    public class UserCredentials
    {
        #region Constructors

        public UserCredentials()
        {
            //
        }

        #endregion

        #region Properties

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        #endregion
    }
}
