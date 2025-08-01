using System.Data;
using System.Data.SQLite;

namespace LoungeSaber_Server.SQL;

public abstract class Database
{
    protected abstract string DatabaseName { get; }
    
    protected readonly SQLiteConnection Connection;

    protected static string DataFolderPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

    public bool IsOpen => Connection.State == ConnectionState.Open;

    protected Database()
    {
        Connection = new($"Data Source={Path.Combine(DataFolderPath, $"{DatabaseName}.db;")}");
    }

    public void Start()
    {
        if (IsOpen) 
            return;

        Directory.CreateDirectory(DataFolderPath);
        
        Connection.Open();
        CreateInitialTables();
    }
    
    public void Stop()
    {
        if (!IsOpen) 
            return;
        
        Connection.Close();
    }

    protected abstract void CreateInitialTables();
}