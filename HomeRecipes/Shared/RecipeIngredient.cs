namespace HomeRecipes.Shared
{
    public class RecipeIngredient
    {
        public string Name { get; set; }

        public double Quantity { get; set; } = 1;

        public string Units { get; set; }

        public string PrepGroup { get; set; }
    }
}
