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

    public async Task<IEnumerable<Recipe>> GetAllByUserIdAsync(int userId)
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
                    ,ri.UnitTypeCd
                    ,ri.Quantity
                    ,ri.Notes
                    ,ri.DisplayOrder
                    ,ri.CreateDttm
                    ,ri.UpdateDttm
                    ,i.IngredientId
                    ,i.IngredientName
                    ,i.IsExotic
                    ,i.IsPerishable
                    ,ut.UnitTypeCd
                    ,ut.UnitDescription
                FROM public.Recipe r
                    INNER JOIN public.AppUser u ON r.UserId = u.UserId
                    LEFT JOIN public.RecipeIngredient ri ON r.RecipeId = ri.RecipeId
                    LEFT JOIN public.Ingredient i ON ri.IngredientId = i.IngredientId
                    LEFT JOIN reference.UnitType ut on ri.UnitTypeCd = ut.UnitTypeCd
                WHERE r.RecipeId = @RecipeId AND u.UserId = @UserId
                ORDER BY ri.DisplayOrder";

            var recipeDictionary = new Dictionary<int, Recipe>();

            await conn.QueryAsync<Recipe, User, RecipeIngredient, Ingredient, UnitType, Recipe>(
                sql,
                (recipe, user, recipeIngredient, ingredient, unitType) =>
                {
                    if (!recipeDictionary.TryGetValue(recipe.RecipeId, out var recipeEntry))
                    {
                        recipeEntry = recipe;
                        recipeEntry.User = user;
                        recipeEntry.RecipeIngredients.Add(recipeIngredient);
                        recipeDictionary.Add(recipe.RecipeId, recipeEntry);
                    }

                    if (recipeIngredient != null && ingredient != null && unitType != null)
                    {
                        recipeIngredient.Ingredient = ingredient;
                        recipeIngredient.UnitType = unitType;
                        recipeEntry.RecipeIngredients.Add(recipeIngredient);
                    }

                    return recipeEntry;
                },
                new { RecipeId = recipeId, UserId = userId },
                splitOn: "UserId,RecipeId,IngredientId,UnitTypeCd"
            );
            var result = recipeDictionary.Values.FirstOrDefault();

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
                            @"INSERT INTO RecipeIngredient (RecipeId, IngredientId, UnitTypeCd, Quantity, Notes, DisplayOrder)
                          VALUES (@RecipeId, @IngredientId, @UnitTypeCd, @Quantity, @Notes, @DisplayOrder)",
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
            var rowsAffected = await conn.ExecuteAsync(
                @"UPDATE recipe 
                  SET UserId = @UserId
                  , RecipeName = @RecipeName
                  , RecipeDescription = @RecipeDescription
                  , PrepTimeMinutes = @PrepTimeMinutes
                  , CookTimeMinutes = @CookTimeMinutes
                  , Servings = @Servings
                  WHERE RecipeId = @RecipeId AND UserId = @UserId",
                new
                {
                    UserId = userId,
                    request.RecipeName,
                    request.RecipeDescription,
                    request.PrepTimeMinutes,
                    request.CookTimeMinutes,
                    request.Servings
                }
            );

            return rowsAffected > 0;
        }
    }

    public async Task<bool> DeleteAsync(int recipeId, int userId)
    {
        using (var conn = _connection.GetConnection())
        {
            var rowsAffected = await conn.ExecuteAsync(
                @"
            DELETE FROM Recipe
            WHERE RecipeId = @RecipeId AND UserId = @UserId",
                new { RecipeId = recipeId, UserId = userId }
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
