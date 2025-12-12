using Dapper;
using ProcrastiPlate.Core.Models;
using ProcrastiPlate.Api.Repositories.Interfaces;
using ProcrastiPlate.Server.Configuration;

namespace ProcrastiPlate.Api.Repositories;

public class ProcrastiPlateRepository : IProcrastiPlateRepository
{
    private readonly IDbConnectionFactory _connection;
    public ProcrastiPlateRepository(IDbConnectionFactory connection)
    {
        _connection = connection;
    }
    public Author GetAuthor(int id)
    {
        using var conn = _connection.GetConnection();
        return conn.QueryFirst<Author>(
            "SELECT * FROM Author " +
            "WHERE AuthorId = @AuthorId;",
            new { AuthorId = id }
        );
    }
    //public async Task<IEnumerable<Recipe>> GetAllRecipesAsync(int userId)
    //{
    //    using var conn = _connection.GetConnection();
    //    return conn.QueryAsync<Author>(
    //@"SELECT * 
    //FROM Recipe 
    //WHERE UserId = @UserId 
    //ORDER BY CreateDttm DESC",
    //        new { UserId = userId }
    //    );
    //}
}
