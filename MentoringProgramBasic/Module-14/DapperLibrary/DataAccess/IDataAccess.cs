using System.Data;

namespace DapperLibrary.DataAccess;

public interface IDataAccess
{
    IDbConnection GetConnection();
    string ConnectionString { get; }
    T Execute<T>(string query, object? parameters) where T : class;
    void Execute(string query, object? parameters);
    IEnumerable<T> Query<T>(string query, object? parameters) where T : class;
    T QuerySingleOrDefault<T>(string query, object? parameters);
}
