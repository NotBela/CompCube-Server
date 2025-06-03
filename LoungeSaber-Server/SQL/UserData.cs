using LoungeSaber_Server.Models.Badge;
using LoungeSaber_Server.Models.Client;

namespace LoungeSaber_Server.SQL;

public class UserData : Database
{
    public static readonly UserData Instance = new();

    protected override string DatabaseName => "LoungeData";

    public UserInfo? GetUser(string userId)
    {
        var command = _connection.CreateCommand();
        command.CommandText = $"SELECT * FROM userData WHERE userData.id = @id LIMIT 1";
        using var reader = command.ExecuteReader();
        
        while (reader.Read())
        {
            if (reader.FieldCount == 0) 
                return null;
            
            var mmr = reader.GetInt32(1);
            var userName = reader.GetString(2);
            var badge = GetBadgeForUser(userId);
            return new UserInfo(userName, userId, mmr, badge);
        }

        return null;
    }

    public Badge? GetBadgeForUser(string userId)
    {
        var command = _connection.CreateCommand();
        command.CommandText = $"SELECT badge FROM badges WHERE id = @id LIMIT 1";
        command.Parameters.AddWithValue("id", userId);
        if (command.ExecuteScalar() == null) 
            return null;
        
        // make badge manager and badge config system
        return null;
    }
    
    public UserInfo UpdateUserLoginData(string userId, string userName)
    {
        // also make sure to update last used username!
        
        var user = GetUser(userId);
        if (user != null) return user;
        
        var addToMmrTable = _connection.CreateCommand();
        addToMmrTable.CommandText = "INSERT INTO userData VALUES (@userId, 1000, @userName)";
        addToMmrTable.Parameters.AddWithValue("userId", userId);
        addToMmrTable.Parameters.AddWithValue("userName", userName);
        addToMmrTable.ExecuteNonQuery();

        return new UserInfo(userName, userId, 1000, null);
    }
    
    protected override void CreateInitialTables()
    {
        CreateUserDataTable();
        CreateLinkedDiscordTable();
        CreateBadgeTable();
    }

    private void CreateLinkedDiscordTable()
    {
        var command = _connection.CreateCommand();
        command.CommandText = "CREATE TABLE IF NOT EXISTS discord (id TEXT NOT NULL PRIMARY KEY, discordId TEXT NOT NULL)";
        command.ExecuteNonQuery();
    }

    private void CreateBadgeTable()
    {
        var command = _connection.CreateCommand();
        command.CommandText = "CREATE TABLE IF NOT EXISTS badges (id TEXT NOT NULL PRIMARY KEY, badge TEXT)";
        command.ExecuteNonQuery();
    }
    
    private void CreateUserDataTable()
    {
        var dbCommand = _connection.CreateCommand();
        dbCommand.CommandText = "CREATE TABLE IF NOT EXISTS userData (id TEXT NOT NULL PRIMARY KEY, mmr INTEGER NOT NULL, username TEXT NOT NULL)";
        dbCommand.ExecuteNonQuery();
    }
}