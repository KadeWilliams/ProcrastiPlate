using Dapper;
using ProcrastiPlate.Core.Configuration;
using ProcrastiPlate.Core.Interfaces.Repositories;
using ProcrastiPlate.Core.Models;

namespace ProcrastiPlate.Infrastructure.Repositories;

public class IngredientRepository : IIngredientRepository
{
    private readonly IDbConnectionFactory _connection;
    public IngredientRepository(IDbConnectionFactory connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<Ingredient>> GetAllIngredientsAsync()
    {
        using (var conn = _connection.GetConnection())
        {
            const string allIngredientsSQL = @"
                SELECT * 
                FROM reference.Ingredient";

            return await conn.QueryAsync<Ingredient>(allIngredientsSQL);
        }
    }

    public async Task<IEnumerable<UserIngredient>> GetAllUserIngredientsByIdAsync(int userId)
    {
        using (var conn = _connection.GetConnection())
        {
            const string userIngredientsSQL = @"
                SELECT * 
                FROM UserIngredient
                WHERE UserId = @UserId";

            return await conn.QueryAsync<UserIngredient>(
                userIngredientsSQL,
                new
                {
                    UserId = userId
                }
            );
        }
    }

}
