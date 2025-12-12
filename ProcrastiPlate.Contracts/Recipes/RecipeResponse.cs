namespace ProcrastiPlate.Contracts.Recipes;

public record RecipeResponse
(
    int RecipeId,
    string RecipeName,
    string? RecipeDescription,
    int PrepTimeMinutes,
    int CookTimeMinutes,
    int Servings
);
