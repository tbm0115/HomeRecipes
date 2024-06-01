using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HomeRecipes.Shared
{
    public class RecipeIngredient
    {
        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [Required]
        [JsonPropertyName("quantity")]
        public double Quantity { get; set; } = 1;

        [JsonPropertyName("units")]
        public string Units { get; set; }

        [JsonPropertyName("prepGroup")]
        public string PrepGroup { get; set; }
    }
}
