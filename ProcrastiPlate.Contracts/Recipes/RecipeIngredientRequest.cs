using System.ComponentModel.DataAnnotations;
namespace ProcrastiPlate.Contracts.Recipes;

public record RecipeIngredientRequest(
    int? IngredientId, // for global ingredients
    int? UserIngredientId, // for custom ingredients
    [Required, MaxLength(20)] string UnitTypeCd,
    [Range(0.001, 10000)] decimal Quantity,
    [MaxLength(500)] string? Notes,
    [Range(0, 1000)] int DisplayOrder = 0
)
{
    // Validation: Must specify one or the other 
    public bool IsValid() => (IngredientId.HasValue && !UserIngredientId.HasValue) || (UserIngredientId.HasValue && !IngredientId.HasValue);
}
