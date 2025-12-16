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
        public DateTime CreateDttm { get; set; }
        public DateTime UpdateDttm { get; set; }

        // Navigation properties 
        public User User { get; set; } = null!;
        public List<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
        public List<RecipeStep> RecipeSteps { get; set; } = new List<RecipeStep>();

        // Computed properties
        public int TotalTimeMinutes => (PrepTimeMinutes ?? 0) + (CookTimeMinutes ?? 0);
        public bool HasSteps => RecipeSteps.Any();
    }
}
