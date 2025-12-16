using ProcrastiPlate.Contracts.Recipes;

namespace ProcrastiPlate.Core.Interfaces.Services;

public interface IRecipeService
{
    Task<IEnumerable<RecipeListResponse>> GetRecipesListByUserIdAsync(int userId);
    Task<RecipeDetailResponse> CreateRecipeAsync(CreateRecipeRequest request, int userId);
    Task<RecipeDetailResponse> UpdateRecipeAsync(int recipeId, UpdateRecipeRequest request, int userId);
    Task<bool> DeleteRecipeAsync(int recipeId, int userId);
}
