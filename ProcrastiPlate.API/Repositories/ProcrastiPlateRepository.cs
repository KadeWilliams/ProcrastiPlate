using Dapper;
using ProcrastiPlate.Server.Configuration;
using ProcrastiPlate.API.Models;
using ProcrastiPlate.API.Repositories.Interfaces;
using ProcrastiPlate.Server.Configuration;

namespace ProcrastiPlate.API.Repositories;

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
}