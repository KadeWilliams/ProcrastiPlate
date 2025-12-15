using ProcrastiPlate.Contracts.Recipes;

namespace ProcrastiPlate.Core.Interfaces.Services;

public interface IRecipeService
{
    Task<RecipeDetailResponse> CreateRecipeAsync(CreateRecipeRequest request, int userId);
    Task<RecipeDetailResponse> UpdateRecipeAsync(UpdateRecipeRequest request, int userId);
}
