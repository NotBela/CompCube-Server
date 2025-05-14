using Microsoft.Data.Sqlite;

namespace LoungeSaber_Server.SQL;

public class MapData : Database
{
    public static readonly MapData Instance = new();
    
    protected override string DatabaseName => "MapData";
    
    protected override void CreateInitialTable()
    {
        var createDBCommand = _connection.CreateCommand();
        createDBCommand.CommandText = "CREATE TABLE IF NOT EXISTS mapData ( " + 
                                      "hash TEXT NOT NULL PRIMARY KEY, " + 
                                      "difficulty TEXT NOT NULL, " + 
                                      "characteristic TEXT NOT NULL, " + 
                                      "category TEXT NOT NULL " +
                                      ");";
        createDBCommand.ExecuteNonQuery();
    }
}