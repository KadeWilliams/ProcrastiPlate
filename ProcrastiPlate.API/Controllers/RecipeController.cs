using Microsoft.AspNetCore.Mvc;
using ProcrastiPlate.Core.Models;
using ProcrastiPlate.Api.Models.DTOs;
using ProcrastiPlate.Api.Repositories.Interfaces;

namespace ProcrastiPlate.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipesController : ControllerBase
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly ILogger<RecipesController> _logger;

    public RecipesController(IRecipeRepository recipeRepository, ILogger<RecipesController> logger)
    {
        _recipeRepository = recipeRepository;
        _logger = logger;
    }

    // GET: api/recipes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipes()
    {
        try
        {
            // TODO: Get real user ID from auth token
            // For now, hard code to test user

            var userId = 1;

            var recipes = await _recipeRepository.GetAllRecipesAsync(userId);
            return Ok(recipes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recipes");
            return StatusCode(500, "An error occurred while retrieving recipes");
        }
    }

    // GET: api/recipes/5
    [HttpGet("{recipeId}")]
    public async Task<ActionResult<Recipe>> GetRecipe(int recipeId)
    {
        try
        {
            // TODO: Get real user ID from auth token
            var userId = 1;

            var recipe = await _recipeRepository.GetRecipeByIdAsync(recipeId, userId);
            if (recipe == null)
            {
                return NotFound($"Recipe with ID {recipeId} not found");
            }
            return Ok(recipe);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recipe {RecipeID}", recipeId);
            return StatusCode(500, "An error occurred while retrieving recipe");
        }
    }

    // POST: api/recipes
    [HttpPost]
    public async Task<ActionResult<Recipe>> CreateRecipe([FromBody] CreateRecipeRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // TODO: Get real user ID from auth token
            var userId = 1;

            var recipe = await _recipeRepository.CreateRecipeAsync(request, userId);
            return CreatedAtAction(nameof(GetRecipe), new { id = recipe.RecipeId }, recipe);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating recipe");
            return StatusCode(500, "An error occurred while creating the recipe");
        }
    }

    // POST: api/recipes/5
    [HttpPut("{recipeId}")]
    public async Task<ActionResult<Recipe>> UpdateRecipe(
        int recipeId,
        [FromBody] UpdateRecipeRequest request
    )
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // TODO: Get real user ID from auth token
            var userId = 1;

            var success = await _recipeRepository.UpdateRecipeAsync(recipeId, request, userId);

            if (!success)
            {
                return NotFound($"Recipe with ID {recipeId} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating recipe {RecipeId}", recipeId);
            return StatusCode(500, "An error occurred while updating the recipe");
        }
    }

    // POST: api/recipes/5
    [HttpDelete("{recipeId}")]
    public async Task<ActionResult<Recipe>> DeleteRecipe(int recipeId)
    {
        try
        {
            // TODO: Get real user ID from auth token
            var userId = 1;

            var success = await _recipeRepository.DeleteRecipeAsync(recipeId, userId);

            if (!success)
            {
                return NotFound($"Recipe with ID {recipeId} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting recipe {RecipeId}", recipeId);
            return StatusCode(500, "An error occurred while deleting the recipe");
        }
    }
}
