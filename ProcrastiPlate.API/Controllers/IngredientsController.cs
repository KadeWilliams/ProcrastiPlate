using Microsoft.AspNetCore.Mvc;
using ProcrastiPlate.Contracts.Ingredients;
using ProcrastiPlate.Core.Interfaces.Services;

namespace ProcrastiPlate.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IngredientsController : ControllerBase
{
    private readonly IIngredientService _ingredientService;
    private readonly ILogger<IngredientsController> _logger;

    public IngredientsController(IIngredientService ingredientService, ILogger<IngredientsController> logger)
    {
        _ingredientService = ingredientService;
        _logger = logger;
    }

    // GET: api/ingredients/1 
    [HttpGet("{userId}")]
    public async Task<ActionResult<IEnumerable<IngredientResponse>>> GetAllIngredientsAsync(int userId)
    {
        // todo do the actual authentication
        userId = 1;

        var userIngredients = await _ingredientService.GetAllIngredientsAsync(userId);

        return Ok(userIngredients);
    }
}
