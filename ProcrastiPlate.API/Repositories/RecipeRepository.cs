using Dapper;
using ProcrastiPlate.Api.Models;
using ProcrastiPlate.Api.Models.DTOs;
using ProcrastiPlate.Api.Repositories.Interfaces;
using ProcrastiPlate.Server.Configuration;

namespace ProcrastiPlate.Api.Repositories;

public class RecipeRepository : IRecipeRepository
{
    private readonly IDbConnectionFactory _connection;

    public RecipeRepository(IDbConnectionFactory connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<Recipe>> GetAllRecipesAsync(int userId)
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

    public async Task<Recipe?> GetRecipeByIdAsync(int recipeId, int userId)
    {
        using (var conn = _connection.GetConnection())
        {
            var recipe = await conn.QueryFirstOrDefaultAsync<Recipe>(
                @"SELECT * 
                FROM Recipe 
                WHERE RecipeId = @RecipeId 
                AND UserId = @UserId 
                ORDER BY CreateDttm DESC",
                new { RecipeId = recipeId, UserId = userId }
            );

            if (recipe == null)
                return null;

            recipe.Ingredients = (
                await conn.QueryAsync<RecipeIngredientDetail>(
                    @"SELECT 
                        ri.IngredientId,
                        IngredientName,
                        Quantity, 
                        ut.UnitTypeCd,
                        UnitDescription, 
                        Notes, 
                        DisplayOrder
                    FROM 
                        RecipeIngredient ri 
                            INNER JOIN Ingredient i ON ri.IngredientId = i.IngredientId 
                            INNER JOIN reference.UnitType ut on ri.UnitTypeCd = ut.UnitTypeCd
                        WHERE ri.RecipeId = @RecipeId
                    ORDER BY ri.DisplayOrder
                    ",
                    new { RecipeId = recipeId, UserId = userId }
                )
            ).ToList();
            return recipe;
        }
    }

    public async Task<Recipe> CreateRecipeAsync(CreateRecipeRequest request, int userId)
    {
        using (var conn = _connection.GetConnection())
        {
            try
            {
                var recipeId = await conn.ExecuteScalarAsync<int>(
                    @"INSERT INTO Recipe (UserId, Name, Description, PrepTimeMinutes, CookTimeMinutes, Servings)
                  VALUES (@UserId, @Name, @Description, @PrepTimeMinutes, @CookTimeMinutes, @Servings)
                  RETURNING RecipeId
                ",
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

                if (request.Ingredients?.Any() == true)
                {
                    foreach (var ingredient in request.Ingredients)
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
                return (await GetRecipeByIdAsync(recipeId, userId))!;
            }
            catch
            {
                throw;
            }
        }
    }

    public async Task<bool> UpdateRecipeAsync(int recipeId, UpdateRecipeRequest request, int userId)
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

    public async Task<bool> DeleteRecipeAsync(int recipeId, int userId)
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
}
