using System.Data;
using MySqlConnector;

namespace CompCube_Server.SQL;

public abstract class TableManager : IDisposable
{
    protected static MySqlConnection Connection = new();

    public bool IsOpen => Connection.State == ConnectionState.Open;

    protected TableManager(IConfiguration configuration)
    {
        if (IsOpen) 
            return;
        var connectionString = configuration.GetSection("ConnectionStrings").GetValue<string>("DefaultConnection");
        
        Connection = new MySqlConnection(connectionString);
        
        Connection.Open();
        
        
    }
    
    protected abstract void CreateInitialTables();

    public void Dispose()
    {
        Connection.Dispose();
    }
}