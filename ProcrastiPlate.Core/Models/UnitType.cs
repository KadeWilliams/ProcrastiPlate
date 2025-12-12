namespace ProcrastiPlate.Core.Models;

public class UnitType
{
    public string UnitTypeCd { get; set; } = string.Empty;
    public string UnitTypeDescription { get; set; } = string.Empty;
    public DateTime CreateDttm { get; set; }

    // Navigation properties
    public IEnumerable<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
}
