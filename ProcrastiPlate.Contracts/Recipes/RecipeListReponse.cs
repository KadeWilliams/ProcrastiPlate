namespace ProcrastiPlate.Contracts.Recipes;

public record RecipeListResponse(
    int RecipeId,
    string RecipeName,
    string? RecipeDescription,
    int? TotalTimeMinutes,
    int? Servings,
    //string AuthorName, // User.FullName
    int IngredientCount,
    DateTime CreateDttm
);
