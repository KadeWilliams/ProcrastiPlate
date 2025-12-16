using ProcrastiPlate.Contracts.Recipes;
using ProcrastiPlate.Core.Models;

namespace ProcrastiPlate.Core.Interfaces.Repositories;

public interface IRecipeRepository
{
    #region GET Calls
    Task<IEnumerable<Recipe>> GetAllByIdAsync(int userId);
    Task<Recipe?> GetByIdAsync(int recipeId, int userId, bool includeSteps = false);
    Task<int> GetTotalCountAsync();
    Task<int> GetCountByUserAsync(int userId);
    Task<IEnumerable<Recipe>> SearchAsync(string searchTerm, int page, int pageSize);
    #endregion

    #region POST Calls
    Task<Recipe> CreateAsync(Recipe recipe, int userId);
    #endregion

    #region PUT/DELETE Calls
    Task<bool> UpdateAsync(int recipeId, UpdateRecipeRequest request, int userId);
    Task<bool> DeleteRecipeAsync(int recipeId, int userId);
    Task<bool> DeleteRecipeIngredientAsync(int recipeId, int ingredientId);
    Task<bool> DeleteRecipeStepAsync(int recipeId, int recipeStepId);
    #endregion

}
