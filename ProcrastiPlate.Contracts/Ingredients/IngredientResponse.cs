namespace ProcrastiPlate.Contracts.Ingredients;

public record IngredientResponse(
    int IngredientId,
    string IngredientName,
    bool IsExotic,
    bool IsPerishable
);
