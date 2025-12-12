using Microsoft.AspNetCore.Mvc;
using ProcrastiPlate.Core.Models;
using ProcrastiPlate.Api.Models.DTOs;
using ProcrastiPlate.Api.Repositories;

namespace ProcrastiPlate.Api.Controllers;

public class IngredientController : ControllerBase
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly ILogger _logger;

    public IngredientController(IIngredientRepository ingredientRepository, ILogger logger)
    {
        _ingredientRepository = ingredientRepository;
        _logger = logger;
    }

    // public GetRecipeIngredients(int recipeid)
}
