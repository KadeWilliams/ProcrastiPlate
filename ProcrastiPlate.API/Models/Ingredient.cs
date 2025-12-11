namespace ProcrastiPlate.Api.Models;

public class Ingredient 
{
    public int IngredientId { get; set; }
    public string Name { get; set; }
    public bool IsExotic { get; set; }
    public bool IsPerishable { get; set; }
    public DateTime CreateDttm { get; set; }
    public DateTime UpdateDttm { get; set; }
}
