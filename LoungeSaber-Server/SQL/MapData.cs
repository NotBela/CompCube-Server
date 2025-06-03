namespace LoungeSaber_Server.SQL;

public class MapData : Database
{
    public static readonly MapData Instance = new();
    
    protected override string DatabaseName => "MapData";
    
    protected override void CreateInitialTables()
    {
        var createDbCommand = _connection.CreateCommand();
        createDbCommand.CommandText = "CREATE TABLE IF NOT EXISTS mapData ( " + 
                                      "hash TEXT NOT NULL, " + 
                                      "difficulty TEXT NOT NULL, " + 
                                      "characteristic TEXT NOT NULL, " + 
                                      "category TEXT NOT NULL " +
                                      ");";
        createDbCommand.ExecuteNonQuery();
    }
}