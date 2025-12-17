namespace ProcrastiPlate.Core.Models;

public class UserIngredient
{
    public int UserIngredientId { get; set; }
    public int UserId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public string? Category { get; set; }
    public bool IsExotic { get; set; }
    public bool IsPerishable { get; set; }
    public DateTime CreateDttm { get; set; }

    // might be for navigation
    public User User { get; set; } = null!;
}
