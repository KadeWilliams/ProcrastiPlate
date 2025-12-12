using System.ComponentModel.DataAnnotations;
namespace ProcrastiPlate.Contracts.Recipes;

public record RecipeIngredientRequest(
    int IngredientId,
    [Required, MaxLength(20)] string UnitTypeCd,
    [Range(0.001, 10000)] decimal Quantity,
    [MaxLength(500)] string? Notes,
    [Range(0, 1000)] int DisplayOrder = 0
);
