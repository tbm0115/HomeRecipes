using System;
using System.Text.Json.Serialization;

namespace HomeRecipes.Shared
{
    public class CollectionConfig
    {
        /// <summary>
        /// Name of the collection which ends up being the filename.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The Google Drive File ID.
        /// </summary>
        [JsonPropertyName("fieldId")]
        public string FileId { get; set; }

        /// <summary>
        /// Reference to the last time the Google Drive File was updated.
        /// </summary>
        [JsonPropertyName("dateModifiedUtc")]
        public DateTime DateModifiedUtc { get; set; }

    }
}
