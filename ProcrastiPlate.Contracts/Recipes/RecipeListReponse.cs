namespace ProcrastiPlate.Contracts.Recipes;

public record RecipeListReponse(
    int RecipeId,
    string RecipeName,
    string? RecipeDescription,
    int? TotalTimeMinutes,
    int? Servings,
    string AuthorName, // User.FullName
    int IngredientCount,
    DateTime CreatedDttm
);
