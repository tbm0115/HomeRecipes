using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HomeRecipes.Shared
{
    /// <summary>
    /// Configuration for a collection of recipes.
    /// </summary>
    public class RecipeCollection
    {
        /// <summary>
        /// The name of this collection.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The description of this collection.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// The collection of recipes.
        /// </summary>
        [JsonPropertyName("recipes")]
        public List<Recipe> Recipes { get; set; } = new List<Recipe>();

        /// <summary>
        /// Collection of collaborators for the collection.
        /// </summary>
        [JsonPropertyName("collaborators")]
        public List<Collaborator> Collaborators { get; set; } = new List<Collaborator>();

        /// <summary>
        /// Date that the collection was first created.
        /// </summary>
        [JsonPropertyName("dateCreatedUtc")]
        public DateTime DateCreatedUtc { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date that the collection was last modified.
        /// </summary>
        [JsonPropertyName("dateModifiedUtc")]
        public DateTime DateModifiedUtc { get; set; } = DateTime.UtcNow;
    }
}
