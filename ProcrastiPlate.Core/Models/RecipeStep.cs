namespace ProcrastiPlate.Core.Models
{
    public class RecipeStep
    {
        public int RecipeStepId { get; set; }
        public int RecipeId { get; set; }
        public int StepNumber { get; set; }
        public string Instruction { get; set; }
        public int DurationMinutes { get; set; }
        public int Temperature { get; set; }
        public char TemperatureUnit { get; set; }
        public byte[]? StepImage { get; set; }

        // Navigation properties 
        public Recipe Recipe { get; set; } = null!;
    }
}