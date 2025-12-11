namespace ProcrastiPlate.Api.Models
{
    public class Recipe
    {
        public int RecipeId { get; set; }
        public int UserId { get; set; }
        public string RecipeName { get; set; }
        public string? RecipeDescription { get; set; }
        public int? PrepTimeMinutes { get; set; }
        public int? CookTimeMinutes { get; set; }
        public int? Servings { get; set; }
        public string? Instructions { get; set; }
        public DateTime CreatedDttm { get; set; }
        public DateTime UpdatedDttm { get; set; }

        public IEnumerable<RecipeIngredientDetail>? Ingredients { get; set; }
    }
}
