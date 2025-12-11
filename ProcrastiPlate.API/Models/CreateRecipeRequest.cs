namespace ProcrastiPlate.Api.Models.DTOs;

// todo: create a new class for creating new recipe steps
public class CreateRecipeRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? PrepTimeMinutes { get; set; }
    public int? CookTimeMinutes { get; set; }
    public int? Servings { get; set; }
    public IEnumerable<CreateRecipeIngredientRequest>? Ingredients { get; set; }
}

public class CreateRecipeIngredientRequest
{
    public int IngredientId { get; set; }
    public string UnitTypeCd { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? Notes { get; set; }
    public int DisplayOrder { get; set; }
}

public class UpdateRecipeRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? PrepTimeMinutes { get; set; }
    public int? CookTimeMinutes { get; set; }
    public int? Servings { get; set; }
}
