using System.Text.Json.Serialization;

namespace HomeRecipes.Shared
{
    /// <summary>
    /// Reference to another user that can collaborate on Google Drive File(s).
    /// </summary>
    public class Collaborator
    {
        /// <summary>
        /// Email address of the user allowed to collaborate on this Google Drive File.
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; }

        /// <summary>
        /// Role that this specific collaborator has on this Google Drive File.
        /// </summary>
        [JsonPropertyName("role")]
        public string Role { get; set; } // "editor" or "viewer"
    }
}
