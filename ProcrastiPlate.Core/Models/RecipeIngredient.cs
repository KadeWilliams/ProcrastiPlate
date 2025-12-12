namespace ProcrastiPlate.Core.Models;

public class RecipeIngredient
{
    public int RecipeId { get; set; }
    public int IngredientId { get; set; }
    public string UnitTypeCd { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? Notes { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreateDttm { get; set; }

    // Navigation properties
    public Recipe Recipe { get; set; } = null!;
    public Ingredient Ingredient { get; set; } = null!;
    public UnitType UnitType { get; set; } = null!;
}
