using System.Data;
using System.Data.SqlClient;

namespace AdoNetLibrary.DataAccess;

public interface IDataAccess
{
    SqlConnection GetConnection();
    void ExecuteNonQuery(string commandText, CommandType commandType = CommandType.Text, SqlParameter[]? parameters = null);
    T ExecuteScalar<T>(string commandText, CommandType commandType = CommandType.Text, SqlParameter[]? parameters = null);
    DataTable ExecuteQuery(string commandText, CommandType commandType = CommandType.Text, SqlParameter[]? parameters = null);
}
