namespace ProcrastiPlate.Contracts.Recipes;

public record CreateRecipeIngredientRequest(
    int IngredientId,
    string UnitTypeCd,
    decimal Quantity,
    string? Notes,
    int DisplayOrder
);


