using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProcrastiPlate.Contracts.Recipes;
using ProcrastiPlate.Core.Interfaces.Repositories;
using ProcrastiPlate.Core.Interfaces.Services;
using ProcrastiPlate.Core.Models;

namespace ProcrastiPlate.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipesController : ControllerBase
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IRecipeService _recipeService;
    private readonly ILogger<RecipesController> _logger;

    public RecipesController(
        IRecipeRepository recipeRepository,
        IRecipeService recipeService,
        ILogger<RecipesController> logger
    )
    {
        _recipeRepository = recipeRepository;
        _recipeService = recipeService;
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

            var recipes = await _recipeRepository.GetAllRecipesByUserIdAsync(userId);
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

            var recipe = await _recipeRepository.GetByIdAsync(recipeId, userId);
            if (recipe == null)
            {
                return NotFound($"Recipe with ID {recipeId} not found");
            }
            return Ok(recipe);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recipe {RecipeID}", recipeId);
            return StatusCode(500, $"An error occurred while retrieving recipe");
        }
    }

    // POST: api/recipes
    /// <summary>
    /// Create a new recipe
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(RecipeDetailResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<RecipeDetailResponse>> CreateRecipe([FromBody] CreateRecipeRequest request)
    {
        /*
            What happens here:
            1. ASP.NET checks JWT token (due to [Authorize])
            2. If token is valid, creates User.Claims with userId, email, etc.
            3. Deserializes JSON body into CreateRecipeRequest object
            4. Validates data annotations ([Required], [MaxLength], etc.)
        */

        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // TODO: Get real user ID from auth token
            // Extract user ID from JWT token
            // var userId = User.GetUserId(); // from claims: "123"
            var userId = 1;

            // pass to service layer - we're NOT dealing with HTTP stuff 
            var recipe = await _recipeService.CreateRecipeAsync(request, userId);

            // return HTTP 201 with Created location header
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

            var success = await _recipeService.UpdateRecipeAsync(request, userId);

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
