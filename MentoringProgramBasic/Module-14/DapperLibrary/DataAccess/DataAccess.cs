using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DapperLibrary.DataAccess;

public class DataAccess(string connectionString) : IDataAccess
{
    private readonly string _connectionString = ValidateConnectionString(connectionString);

    public string ConnectionString => _connectionString;

    public IDbConnection GetConnection() => new SqlConnection(_connectionString);

    public T Execute<T>(string query, object? parameters) where T : class
    {
        using var connection = GetConnection();
        return connection.QuerySingleOrDefault<T>(query, parameters)!;
    }

    public void Execute(string query, object? parameters)
    {
        using var connection = GetConnection();
        connection.Execute(query, parameters);
    }

    public IEnumerable<T> Query<T>(string query, object? parameters) where T : class
    {
        using var connection = GetConnection();
        return connection.Query<T>(query, parameters);
    }

    private static string ValidateConnectionString(string connectionString)
    {
        if (connectionString == null)
            throw new ArgumentNullException(nameof(connectionString));
        
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string cannot be empty or whitespace.", nameof(connectionString));
        
        return connectionString;
    }

    public T QuerySingleOrDefault<T>(string query, object? parameters)
    {
        using var connection = GetConnection();
        return connection.QuerySingleOrDefault<T>(query, parameters)!;
    }
}
