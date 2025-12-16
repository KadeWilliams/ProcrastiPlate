namespace ProcrastiPlate.Contracts.Recipes;

public record RecipeStepResponse(
    int RecipeStepId,
    int StepNumber,
    string Instruction,
    int? DurationMinutes,
    string? TemperatureUnit,
    string? StepImageUrl //URL to stored image, not raw bytes 
);
