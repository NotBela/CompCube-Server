namespace LoungeSaber_Server.SQL;

public class UserData : Database
{
    public static readonly UserData Instance = new();

    protected override string DatabaseName => "LoungeData";
    
    protected override void CreateInitialTables()
    {
        CreateMmrTable();
        CreateDiscordIdTable();
        CreateUserNameTable();
    }

    private void CreateMmrTable()
    {
        var dbCommand = _connection.CreateCommand();
        dbCommand.CommandText = "CREATE TABLE IF NOT EXISTS mmr (id TEXT PRIMARY KEY NOT NULL, mmr INTEGER NOT NULL)";
        dbCommand.ExecuteNonQuery();
    }

    private void CreateDiscordIdTable()
    {
        var dbCommand = _connection.CreateCommand();
        dbCommand.CommandText =
            "CREATE TABLE IF NOT EXISTS discord (id TEXT PRIMARY KEY NOT NULL, discordId TEXT NOT NULL)";
        dbCommand.ExecuteNonQuery();
    }

    private void CreateUserNameTable()
    {
        var dbCommand = _connection.CreateCommand();
        dbCommand.CommandText = "CREATE TABLE IF NOT EXISTS username (id TEXT NOT NULL PRIMARY KEY, username TEXT NOT NULL)";
        dbCommand.ExecuteNonQuery();
    }
}