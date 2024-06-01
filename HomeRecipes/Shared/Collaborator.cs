using System.Text.Json.Serialization;

namespace HomeRecipes.Shared
{
    public class Collaborator
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; } // "editor" or "viewer"
    }
}
