using System.ComponentModel.DataAnnotations;

namespace ProcrastiPlate.Contracts.Recipes;

public record CreateRecipeRequest(
    [Required, MaxLength(300)] string RecipeName,
    [MaxLength(2000)] string? RecipeDescription,
    [Range(0, 1440)] int? PrepTimeMinutes,
    [Range(0, 1440)] int? CookTimeMinutes,
    [Range(1, 100)] int? Servings,
    [Required, MinLength(1)] List<RecipeIngredientRequest> Ingredients
);


