using ProcrastiPlate.Contracts.Ingredients;

namespace ProcrastiPlate.Core.Interfaces.Services;

public interface IIngredientService
{
    Task<List<IngredientResponse>> GetAllIngredientsAsync(int userId);
}
