using Microsoft.Extensions.Logging;
using ProcrastiPlate.Contracts.Ingredients;
using ProcrastiPlate.Core.Interfaces.Repositories;
using ProcrastiPlate.Core.Interfaces.Services;
using ProcrastiPlate.Core.Models;

namespace ProcrastiPlate.Core.Services;

public class IngredientService : IIngredientService
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly ILogger<IngredientService> _logger;

    public IngredientService(IIngredientRepository ingredientRepository, ILogger<IngredientService> logger)
    {
        _ingredientRepository = ingredientRepository;
        _logger = logger;
    }

    public async Task<List<IngredientResponse>> GetAllIngredientsAsync(int userId)
    {
        var ingredientList = await _ingredientRepository.GetAllIngredientsAsync();
        var userIngredientList = await _ingredientRepository.GetAllUserIngredientsByIdAsync(userId);

        return MapToListIngredientResponse(ingredientList, userIngredientList);
    }

    private List<IngredientResponse> MapToListIngredientResponse(IEnumerable<Ingredient> ingredients, IEnumerable<UserIngredient> userIngredients)
    {
        List<IngredientResponse> ingredientResponseList = new();
        foreach (var ingredient in ingredients)
        {
            var ingredientResponse = new IngredientResponse(
                null,
                ingredient.IngredientId,
                ingredient.IngredientName,
                ingredient.Category,
                ingredient.IsExotic,
                ingredient.IsPerishable
            );
            ingredientResponseList.Add(ingredientResponse);
        }

        foreach (var ingredient in userIngredients)
        {
            var ingredientResponse = new IngredientResponse(
                ingredient.UserIngredientId,
                null,
                ingredient.IngredientName,
                ingredient.Category,
                ingredient.IsExotic,
                ingredient.IsPerishable
            );
            ingredientResponseList.Add(ingredientResponse);
        }
        return ingredientResponseList;
    }
}
