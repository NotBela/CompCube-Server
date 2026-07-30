using System.Data;
using MySqlConnector;

namespace CompCube_Server.SQL;

public abstract class TableManager : IDisposable
{
    protected readonly MySqlConnection Connection;

    protected TableManager(IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("ConnectionStrings").GetValue<string>("DefaultConnection");
        
        Connection = new MySqlConnection(connectionString);
        
        Connection.Open();
        
        CreateInitialTables();
    }
    
    protected abstract void CreateInitialTables();

    public void Dispose()
    {
        Connection.Dispose();
    }
}