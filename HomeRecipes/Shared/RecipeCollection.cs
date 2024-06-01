using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HomeRecipes.Shared
{
    public class RecipeCollection
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("recipes")]
        public List<Recipe> Recipes { get; set; }

        [JsonPropertyName("collaborators")]
        public List<Collaborator> Collaborators { get; set; }
    }
}
