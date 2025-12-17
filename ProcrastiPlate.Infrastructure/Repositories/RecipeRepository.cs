using Dapper;
using ProcrastiPlate.Contracts.Recipes;
using ProcrastiPlate.Core.Configuration;
using ProcrastiPlate.Core.Interfaces.Repositories;
using ProcrastiPlate.Core.Models;

namespace ProcrastiPlate.Api.Repositories;

public class RecipeRepository : IRecipeRepository
{
    private readonly IDbConnectionFactory _connection;

    public RecipeRepository(IDbConnectionFactory connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<Recipe>> GetAllByIdAsync(int userId)
    {
        using (var conn = _connection.GetConnection())
        {
            return await conn.QueryAsync<Recipe>(
                @"SELECT * 
                FROM Recipe 
                WHERE UserId = @UserId 
                ORDER BY CreateDttm DESC",
                new { UserId = userId }
            );
        }
    }
    public async Task<Recipe?> GetByIdAsync(int recipeId, int userId, bool includeSteps = false)
    {
        using (var conn = _connection.GetConnection())
        {
            const string sql = @"
                SELECT 
                    r.RecipeId
                    ,r.UserId
                    ,r.RecipeName
                    ,r.RecipeDescription
                    ,r.PrepTimeMinutes
                    ,r.CookTimeMinutes
                    ,r.Servings
                    ,r.CreateDttm
                    ,r.UpdateDttm
                    ,u.UserId
                    ,u.FirstName
                    ,u.LastName 
                    ,u.Email
                    ,u.CreateDttm
                    ,ri.RecipeId 
                    ,ri.IngredientId
                    ,ri.UserIngredientId
                    ,ri.UnitTypeCd
                    ,ri.Quantity
                    ,ri.Notes
                    ,ri.DisplayOrder
                    ,ri.CreateDttm
                    ,ri.UpdateDttm
                    ,ut.UnitTypeCd
                    ,ut.UnitTypeDescription
                FROM public.Recipe r
                    INNER JOIN public.AppUser u ON r.UserId = u.UserId
                    LEFT JOIN public.RecipeIngredient ri ON r.RecipeId = ri.RecipeId
                    LEFT JOIN reference.UnitType ut on ri.UnitTypeCd = ut.UnitTypeCd
                WHERE r.RecipeId = @RecipeId AND u.UserId = @UserId
                ORDER BY ri.DisplayOrder";

            var recipeDictionary = new Dictionary<int, Recipe>();

            await conn.QueryAsync<Recipe, User, RecipeIngredient, UnitType, Recipe>(
                sql,
                (recipe, user, recipeIngredient, unitType) =>
                {
                    if (!recipeDictionary.TryGetValue(recipe.RecipeId, out var recipeEntry))
                    {
                        recipeEntry = recipe;
                        recipeEntry.User = user;
                        recipeEntry.RecipeIngredients.Add(recipeIngredient);
                        recipeDictionary.Add(recipe.RecipeId, recipeEntry);
                    }

                    if (recipeIngredient != null && unitType != null)
                    {
                        recipeIngredient.UnitType = unitType;
                        recipeEntry.RecipeIngredients.Add(recipeIngredient);
                    }

                    return recipeEntry;
                },
                new { RecipeId = recipeId, UserId = userId },
                splitOn: "UserId,RecipeId,UnitTypeCd"
            );
            var result = recipeDictionary.Values.FirstOrDefault();
            if (result == null) return null;

            result.RecipeIngredients = await GetRecipeIngredientsAsync(recipeId);

            if (result != null && includeSteps)
            {
                const string stepsSql = @"
                    SELECT 
                        RecipeStepId
                        ,RecipeId
                        ,StepNumber
                        ,Instruction
                        ,DurationMinutes
                        ,Temperature
                        ,TemperatureUnit
                    FROM 
                        public.RecipeStep
                    WHERE RecipeId = @RecipeId
                    ORDER BY StepNumber";

                var steps = await conn.QueryAsync<RecipeStep>(
                    stepsSql,
                    new { RecipeId = recipeId }
                );
                result.RecipeSteps = steps.ToList();
            }
            return result;
        }
    }
    private async Task<List<RecipeIngredient>> GetRecipeIngredientsAsync(
        int recipeId
    )
    {
        using (var conn = _connection.GetConnection())
        {
            var recipeIngredients = new List<RecipeIngredient>();

            const string userIngredientsSQL = @"
                SELECT
                    ri.RecipeId
                    , ri.IngredientId
                    , ri.UserIngredientId
                    , ri.UnitTypeCd
                    , ri.Quantity
                    , ri.Notes
                    , ri.DisplayOrder
                    , ri.CreateDttm
                    , ui.UserIngredientId
                    , ui.IngredientName
                    , ui.IsExotic
                    , ui.IsPerishable
                    , ut.UnitTypeCd
                    , ut.UnitTypeDescription
                FROM public.RecipeIngredient ri
                    INNER JOIN public.UserIngredient ui on ri.UserIngredientId = ui.UserIngredientId
                    INNER JOIN reference.UnitType ut on ri.UnitTypeCd = ut.UnitTypeCd
                WHERE ri.RecipeId = @RecipeId
                    AND ri.UserIngredientId IS NOT NULL";

            var userIngredients = await conn.QueryAsync<RecipeIngredient, UserIngredient, UnitType, RecipeIngredient>(
                userIngredientsSQL,
                (recipeIngredient, userIngredient, unitType) =>
                {
                    recipeIngredient.UserIngredient = userIngredient;
                    recipeIngredient.UnitType = unitType;
                    return recipeIngredient;
                },
                new { RecipeId = recipeId },
                splitOn: "UserIngredientId, UnitTypeCd"
            );


            recipeIngredients.AddRange(userIngredients);

            const string ingredientsSQL = @"
                SELECT
                    ri.RecipeId
                    , ri.IngredientId
                    , ri.UserIngredientId
                    , ri.UnitTypeCd
                    , ri.Quantity
                    , ri.Notes
                    , ri.DisplayOrder
                    , ri.CreateDttm
                    , i.IngredientId
                    , i.IngredientName
                    , i.IsExotic
                    , i.IsPerishable
                    , ut.UnitTypeCd
                    , ut.UnitTypeDescription
                FROM public.RecipeIngredient ri
                    INNER JOIN reference.Ingredient i on ri.IngredientId = i.IngredientId
                    INNER JOIN reference.UnitType ut on ri.UnitTypeCd = ut.UnitTypeCd
                WHERE ri.RecipeId = @RecipeId";

            var ingredients = await conn.QueryAsync<RecipeIngredient, Ingredient, UnitType, RecipeIngredient>(
                ingredientsSQL,
                (recipeIngredient, ingredient, unitType) =>
                {
                    recipeIngredient.Ingredient = ingredient;
                    recipeIngredient.UnitType = unitType;
                    return recipeIngredient;
                },
                new { RecipeId = recipeId },
                splitOn: "UserIngredientId, UnitTypeCd"
            );

            recipeIngredients.AddRange(ingredients);

            return recipeIngredients;
        }
    }
    public async Task<Recipe> CreateAsync(Recipe recipe, int userId)
    {
        using (var conn = _connection.GetConnection())
        {
            try
            {
                var recipeId = await conn.ExecuteScalarAsync<int>(
                    @"INSERT INTO Recipe (UserId, RecipeName, RecipeDescription, PrepTimeMinutes, CookTimeMinutes, Servings)
                  VALUES (@UserId, @RecipeName, @RecipeDescription, @PrepTimeMinutes, @CookTimeMinutes, @Servings)
                  RETURNING RecipeId
                ",
                    new
                    {
                        UserId = userId,
                        recipe.RecipeName,
                        recipe.RecipeDescription,
                        recipe.PrepTimeMinutes,
                        recipe.CookTimeMinutes,
                        recipe.Servings
                    }
                );

                if (recipe.RecipeIngredients?.Any() == true)
                {
                    foreach (var ingredient in recipe.RecipeIngredients)
                    {
                        await conn.ExecuteAsync(
                            @"INSERT INTO RecipeIngredient (RecipeId, IngredientId, UserIngredientId, UnitTypeCd, Quantity, Notes, DisplayOrder)
                          VALUES (@RecipeId, @IngredientId, @UserIngredientId, @UnitTypeCd, @Quantity, @Notes, @DisplayOrder)",
                            new
                            {
                                RecipeId = recipeId,
                                ingredient.IngredientId,
                                ingredient.UserIngredientId,
                                ingredient.UnitTypeCd,
                                ingredient.Quantity,
                                ingredient.Notes,
                                ingredient.DisplayOrder
                            }
                        );
                    }
                }
                return (await GetByIdAsync(recipeId, userId))!;
            }
            catch
            {
                throw;
            }
        }
    }
    public async Task<bool> UpdateAsync(int recipeId, UpdateRecipeRequest request, int userId)
    {
        using (var conn = _connection.GetConnection())
        {
            const string updateRecipeSQL = @"UPDATE recipe 
                  SET RecipeName = @RecipeName
                  , RecipeDescription = @RecipeDescription
                  , PrepTimeMinutes = @PrepTimeMinutes
                  , CookTimeMinutes = @CookTimeMinutes
                  , Servings = @Servings
                  WHERE RecipeId = @RecipeId AND UserId = @UserId";

            var rowsAffected = await conn.ExecuteAsync(
                updateRecipeSQL,
                new
                {
                    UserId = userId,
                    RecipeId = recipeId,
                    request.RecipeName,
                    request.RecipeDescription,
                    request.PrepTimeMinutes,
                    request.CookTimeMinutes,
                    request.Servings
                }
            );

            if (request.Ingredients?.Any() == true)
            {
                const string ingredientsSQL = @"
                    UPDATE RecipeIngredient
                    SET   
                        UnitTypeCd = @UnitTypeCd
                        , Quantity = @Quantity
                        , Notes = @Notes
                        , DisplayOrder = @DisplayOrder
                    WHERE RecipeId = @RecipeId AND IngredientId = @IngredientId";


                foreach (var ingredient in request.Ingredients)
                {
                    await conn.ExecuteAsync(
                        ingredientsSQL,
                        new
                        {
                            RecipeId = recipeId,
                            ingredient.IngredientId,
                            ingredient.UnitTypeCd,
                            ingredient.Quantity,
                            ingredient.Notes,
                            ingredient.DisplayOrder
                        }
                    );
                }
            }

            if (request.Steps?.Any() == true)
            {
                const string stepsSQL = @"
                    UPDATE RecipeStep 
                    SET   
                    WHERE RecipeId = @RecipeId";


                foreach (var steps in request.Steps)
                {
                    await conn.ExecuteAsync(
                        stepsSQL,
                        new
                        {

                        }
                    );
                }

            }

            return rowsAffected > 0;
        }
    }
    public async Task<bool> DeleteRecipeAsync(int recipeId, int userId)
    {
        using (var conn = _connection.GetConnection())
        {
            const string deleteRecipeSQL = @"
                DELETE FROM RecipeIngredient
                USING Recipe 
                WHERE RecipeId = @RecipeId AND UserId = @UserId;

                DELETE FROM RecipeSteps 
                USING Recipe 
                WHERE RecipeId = @RecipeId AND UserId = @UserId;

                DELETE FROM Recipe
                WHERE RecipeId = @RecipeId AND UserId = @UserId;";

            var rowsAffected = await conn.ExecuteAsync(
                deleteRecipeSQL,
                new { RecipeId = recipeId, UserId = userId }
            );

            return rowsAffected > 0;
        }
    }
    public async Task<bool> DeleteRecipeIngredientAsync(int recipeId, int ingredientId)
    {
        using (var conn = _connection.GetConnection())
        {
            const string deleteRecipeIngredientSQL = @"
                DELETE FROM RecipeIngredient
                WHERE RecipeId = @RecipeId AND IngredientId = @IngredientId";

            var rowsAffected = await conn.ExecuteAsync(
                deleteRecipeIngredientSQL,
                new { RecipeId = recipeId, IngredientId = ingredientId }
            );
            return rowsAffected > 0;
        }
    }
    public async Task<bool> DeleteRecipeStepAsync(int recipeId, int recipeStepId)
    {
        using (var conn = _connection.GetConnection())
        {
            const string deleteRecipeStepSQL = @"
                DELETE FROM RecipeStep 
                WHERE RecipeId = @RecipeId AND RecipeStepId = @RecipeStepId";

            var rowsAffected = await conn.ExecuteAsync(
                deleteRecipeStepSQL,
                new { RecipeId = recipeId, RecipeStepId = recipeStepId }
            );
            return rowsAffected > 0;
        }
    }
    public Task<int> GetTotalCountAsync()
    {
        throw new NotImplementedException();
    }
    public Task<int> GetCountByUserAsync(int userId)
    {
        throw new NotImplementedException();
    }
    public Task<IEnumerable<Recipe>> SearchAsync(string searchTerm, int page, int pageSize)
    {
        throw new NotImplementedException();
    }
}
