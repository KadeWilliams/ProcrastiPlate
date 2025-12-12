using System.ComponentModel.DataAnnotations;

namespace ProcrastiPlate.Contracts.Ingredients;

public record CreateIngredientRequest(
    [Required, MaxLength(200)] string IngredientName,
    bool IsExotic = false,
    bool IsPerishable = false
);