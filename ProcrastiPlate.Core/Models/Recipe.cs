namespace ProcrastiPlate.Core.Models
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

        // Navigation properties 
        public User User { get; set; } = null!;
        public IEnumerable<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
        public IEnumerable<RecipeStep> RecipeSteps { get; set; } = new List<RecipeStep>();

        // Computed properties
        public int TotalTimeMinutes => (PrepTimeMinutes ?? 0) + (CookTimeMinutes ?? 0);
        public bool HasSteps => RecipeSteps.Any();
    }
}
