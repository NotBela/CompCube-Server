using MySqlConnector;

namespace CompCube_Server.Data;

public class DbSession(IConfiguration configuration)
{
    public MySqlConnection CreateNewConnection()
    {
        var connection = new MySqlConnection(configuration.GetConnectionString("DefaultConnection"));
        connection.Open();
        return connection;
    }
}