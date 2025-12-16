using System.ComponentModel.DataAnnotations;

namespace ProcrastiPlate.Contracts.Recipes;

public record RecipeStepRequest(

    [Range(1, 100)] int StepNumber,
    [Required, MaxLength(2000)] string Instruction,
    [Range(0, 1440)] int DurationMinutes,
    [Range(0, 1000)] int? Temperature,
    [RegularExpression("^[FC]", ErrorMessage = "Temperature unit must be F or C")]
    string? TemperatureUnit,
    string? StepImageBase64
);
