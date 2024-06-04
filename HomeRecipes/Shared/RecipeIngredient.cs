using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HomeRecipes.Shared
{
    /// <summary>
    /// Represents an individual ingredient callout
    /// </summary>
    public class RecipeIngredient
    {
        /// <summary>
        /// The name of the ingredient. Ie, Onion
        /// </summary>
        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The quantity of the ingredient, with respect to the <see cref="Units"/>.
        /// </summary>
        [Required]
        [JsonPropertyName("quantity")]
        public double Quantity { get; set; } = 1;

        /// <summary>
        /// The units of the ingredient.
        /// </summary>
        [JsonPropertyName("units")]
        public string Units { get; set; }

        /// <summary>
        /// The preperation group that this ingredient can be joined with other ingredients.
        /// </summary>
        [JsonPropertyName("prepGroup")]
        public string PrepGroup { get; set; }
    }
}
