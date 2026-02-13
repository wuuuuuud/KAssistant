using System.Text.Json.Serialization;

namespace KAssistant.Models
{
    public class AppSettings
    {
        [JsonPropertyName("serverUrl")]
        public string ServerUrl { get; set; } = "http://localhost:5000";

        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

        [JsonPropertyName("rememberCredentials")]
        public bool RememberCredentials { get; set; } = false;
    }
}
