namespace ProcrastiPlate.Contracts.Recipes;

public record RecipeDetailResponse
(
    int RecipeId,
    string RecipeName,
    string? RecipeDescription,
    int? PrepTimeMinutes,
    int? CookTimeMinutes,
    int? TotalTimeMinutes,
    int? Servings,
    UserSummaryResponse Author,
    List<RecipeIngredientResponse> Ingredients,
    List<RecipeStepResponse> Steps,
    DateTime CreateDttm,
    DateTime UpdateDttm
);
