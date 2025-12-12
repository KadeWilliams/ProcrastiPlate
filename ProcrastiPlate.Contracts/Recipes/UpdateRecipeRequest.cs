using System.ComponentModel.DataAnnotations;

namespace ProcrastiPlate.Contracts.Recipes;

public record UpdateRecipeRequest(
    [MaxLength(300)] string? RecipeName,
    [MaxLength(2000)] string? RecipeDescription,
    [Range(0, 1440)] int? PrepTimeMinutes,
    [Range(0, 1440)] int? CookTimeMinutes,
    [Range(1, 100)] int? Servings,
    List<RecipeIngredientRequest>? Ingredients, // if provided replaces all
    List<RecipeStepRequest>? Steps // if provided replaces all
);

