namespace ProcrastiPlate.Contracts.Ingredients;
public record IngredientResponse(
    int? UserIngredientId,
    int? IngredientId,
    string IngredientName,
    string? Category,
    bool IsExotic,
    bool IsPerishable
);
