namespace ProcrastiPlate.Contracts.Recipes;

public record RecipeIngredientResponse(
    int? IngredientId,
    int? UserIngredientId,
    string IngredientName,
    decimal Quantity,
    string UnitTypeCd,
    string UnitDescription,
    string? Notes,
    int DisplayOrder,
    bool IsExotic,
    bool IsPerishable
);
