using MySqlConnector;

namespace CompCube_Server.SQL;

public class DbSession
{
    public readonly MySqlConnection Connection;

    public DbSession(IConfiguration configuration)
    {
        Connection = new MySqlConnection(configuration.GetConnectionString("DefaultConnection"));
        Connection.Open();
    }
}