using ProcrastiPlate.Core.Models;
using ProcrastiPlate.Api.Models.DTOs;

namespace ProcrastiPlate.Core.Interfaces.Repositories;

public interface IRecipeRepository
{
    Task<IEnumerable<Recipe>> GetAllRecipesAsync(int userId);
    Task<Recipe?> GetRecipeByIdAsync(int recipeId, int userId);
    Task<Recipe> CreateRecipeAsync(CreateRecipeRequest request, int userId);
    Task<bool> UpdateRecipeAsync(int recipeId, UpdateRecipeRequest request, int userId);
    Task<bool> DeleteRecipeAsync(int recipeId, int userId);
}
