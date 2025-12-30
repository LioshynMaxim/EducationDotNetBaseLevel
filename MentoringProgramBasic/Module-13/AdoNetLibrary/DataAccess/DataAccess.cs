using System.Data;
using System.Data.SqlClient;

namespace AdoNetLibrary.DataAccess;

public class DataAccess(string connectionString) : IDataAccess
{
    private readonly string _connectionString = connectionString;

    public SqlConnection GetConnection() => new(_connectionString);

    public void ExecuteNonQuery(string commandText, CommandType commandType = CommandType.Text, SqlParameter[]? parameters = null)
    {
        using var connection = GetConnection();
        {
            connection.Open();
            using var command = new SqlCommand(commandText, connection);
            {
                command.CommandType = commandType;
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                command.ExecuteNonQuery();
            }
        }
    }

    public T ExecuteScalar<T>(string commandText, CommandType commandType = CommandType.Text, SqlParameter[]? parameters = null)
    {
        using var connection = GetConnection();
        {
            connection.Open();
            using var command = new SqlCommand(commandText, connection);
            {
                command.CommandType = commandType;
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                var result = command.ExecuteScalar();
                return result == null || result == DBNull.Value ? default! : (T)Convert.ChangeType(result, typeof(T));
            }
        }
    }

    public DataTable ExecuteQuery(string commandText, CommandType commandType = CommandType.Text, SqlParameter[]? parameters = null)
    {
        var dataTable = new DataTable();
        using var connection = GetConnection();
        {
            connection.Open();
            using var command = new SqlCommand(commandText, connection);
            {
                command.CommandType = commandType;
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                using var adapter = new SqlDataAdapter(command);
                adapter.Fill(dataTable);
            }
        }
        return dataTable;
    }
}
