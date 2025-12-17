using Microsoft.Extensions.Logging;
using ProcrastiPlate.Contracts.Recipes;
using ProcrastiPlate.Contracts.Users;
using ProcrastiPlate.Core.Interfaces.Repositories;
using ProcrastiPlate.Core.Interfaces.Services;
using ProcrastiPlate.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace ProcrastiPlate.Core.Services;

public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly ILogger<RecipeService> _logger;

    public RecipeService(IRecipeRepository recipeRepository, ILogger<RecipeService> logger)
    {
        _recipeRepository = recipeRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<RecipeListResponse>> GetRecipesListByUserIdAsync(int userId)
    {
        var recipe = await _recipeRepository.GetAllByIdAsync(userId);

        if (recipe == null)
            return null;

        return MapToListResponse(recipe);
    }
    public async Task<RecipeDetailResponse> GetRecipeAsync(int recipeId, int userId, bool includeSteps)
    {
        var recipe = await _recipeRepository.GetByIdAsync(recipeId, userId, includeSteps);
        return MapToDetailResponse(recipe);
    }
    public async Task<RecipeDetailResponse> CreateRecipeAsync(CreateRecipeRequest request, int userId)
    {
        // validation
        if (request.PrepTimeMinutes < 0 || request.CookTimeMinutes < 0)
        {
            throw new ValidationException("Time values cannot be negative");
        }

        if (!request.Ingredients.Any())
        {
            throw new ValidationException("Recipe must have at least one ingredient");
        }

        // validate ingredients exist 
        // probably better to move this off into the ingredient service and then call it before we get in this method 
        //foreach (var ingredientRequest in request.Ingredients)
        //{
        //    var ingredient = await _ingredientService.GetByIdAsync(ingredientRequest.IngredientId);

        //    if (ingredient == null)
        //        throw new ValidationException($"Ingredient with ID {ingredientRequest.IngredientId} not found");
        //}

        // Create recipe
        var recipe = new Recipe
        {
            // from request DTO
            RecipeName = request.RecipeName,
            RecipeDescription = request.RecipeDescription,
            PrepTimeMinutes = request.PrepTimeMinutes,
            CookTimeMinutes = request.CookTimeMinutes,
            Servings = request.Servings,

            // from authentication (not from user input)
            UserId = userId,

            // from server
            CreateDttm = DateTime.UtcNow,
            UpdateDttm = DateTime.UtcNow
        };

        var createdRecipe = await _recipeRepository.CreateAsync(recipe, userId);

        // build junction table records 

        // Add ingredients
        var recipeIngredients = request.Ingredients.Select(ing => new RecipeIngredient
        {
            RecipeId = createdRecipe.RecipeId,
            IngredientId = ing.IngredientId,
            UserIngredientId = ing.UserIngredientId,
            UnitTypeCd = ing.UnitTypeCd,
            Quantity = ing.Quantity,
            Notes = ing.Notes,
            DisplayOrder = ing.DisplayOrder,
            CreateDttm = DateTime.UtcNow
        }).ToList();

        //save all ingredients in one batch
        //await _recipeIngredientRepository.AddIngredientsAsync(createdRecipe.RecipeId, recipeIngredients);

        // get the full recipe back with User, Ingredients, etc. loaded 
        var completedRecipe = await _recipeRepository.GetByIdAsync(createdRecipe.RecipeId, userId, true);

        return MapToDetailResponse(completedRecipe);
    }
    public async Task<RecipeDetailResponse> UpdateRecipeAsync(int recipeId, UpdateRecipeRequest request, int userId)
    {
        var success = await _recipeRepository.UpdateAsync(recipeId, request, userId);
        if (success)
        {
            var updatedRecipe = await _recipeRepository.GetByIdAsync(recipeId, userId, true);
            return MapToDetailResponse(updatedRecipe);
        }
        return null;
    }
    public async Task<bool> DeleteRecipeAsync(int recipeId, int userId)
    {
        return await _recipeRepository.DeleteRecipeAsync(recipeId, userId);
    }
    private RecipeDetailResponse MapToDetailResponse(Recipe? recipe)
    {
        return new RecipeDetailResponse(
            recipe.RecipeId,
            recipe.RecipeName,
            recipe.RecipeDescription,
            recipe.PrepTimeMinutes,
            recipe.CookTimeMinutes,
            recipe.TotalTimeMinutes,
            recipe.Servings,
            new UserSummaryResponse( // <== flatten user object
                recipe.User.UserId,
                recipe.User.Username,
                recipe.User.FullName,
                null
            ),
            recipe.RecipeIngredients.Select(ri => new RecipeIngredientResponse(
                ri.IngredientId,
                ri.UserIngredientId,
                ri.Ingredient.IngredientName ?? ri.UserIngredient.IngredientName,
                ri.Quantity,
                ri.UnitTypeCd,
                ri.UnitType.UnitTypeDescription,
                ri.Notes,
                ri.DisplayOrder,
                ri.Ingredient.IsExotic,
                ri.Ingredient.IsPerishable
            )).ToList(),
            new List<RecipeStepResponse>(), // empty for now
            recipe.CreateDttm,
            recipe.UpdateDttm
        );
    }
    private List<RecipeListResponse> MapToListResponse(IEnumerable<Recipe> recipes)
    {
        var recipeListResponse = new List<RecipeListResponse>();
        foreach (var recipe in recipes)
        {
            var rec = new RecipeListResponse
            (
                recipe.RecipeId,
                recipe.RecipeName,
                recipe.RecipeDescription,
                recipe.TotalTimeMinutes,
                recipe.Servings,
                recipe.RecipeIngredients.Count(),
                recipe.CreateDttm
            );
            recipeListResponse.Add(rec);
        }
        return recipeListResponse;
    }
}
