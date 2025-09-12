using System.Data;
using Npgsql;
namespace ProcrastiPlate.Server.Configuration;
public interface IDbConnectionFactory
{
    IDbConnection GetConnection();
}
public class NpgsqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;
    public NpgsqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }
    public IDbConnection GetConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}