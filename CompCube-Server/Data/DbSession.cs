using MySqlConnector;

namespace CompCube_Server.Data;

public class DbSession(IConfiguration configuration)
{
    private string ConnectionString => configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("No connection string in config!");

    public MySqlConnection CreateNewConnection()
    {
        var connection = new MySqlConnection(ConnectionString);
        connection.Open();
        return connection;
    }
}