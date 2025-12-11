namespace ProcrastiPlate.Api.Models;

public class RecipeIngredient 
{
    public int RecipeId { get; set; }
    public int IngredientId { get; set; }
    public string UnitTypeCd { get; set; }
    public decimal Quantity { get; set; }
    public string? Notes { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreateDttm { get; set; }
}

public class RecipeIngredientDetail 
{
    public int IngredientId { get; set; }
    public string IngredientName { get; set; }
    public decimal Quantity { get; set; }
    public string UnitTypeCd { get; set; }
    public string UnitTypeDescription { get; set; }
    public string? Notes { get; set; }
    public int DisplayOrder { get; set; }
}
