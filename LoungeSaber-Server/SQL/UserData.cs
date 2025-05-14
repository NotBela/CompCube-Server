using System.Data;
using LoungeSaber_Server.Models;
using Microsoft.Data.Sqlite;

namespace LoungeSaber_Server.SQL;

public class UserData : Database
{
    public static readonly UserData Instance = new();
    
    public bool TryGetUserById(string ID, out User? user)
    {
        user = null;
        
        var command = _connection.CreateCommand();
        command.CommandText = $"SELECT * FROM UserData WHERE UserData.id = {ID}";
        using var reader = command.ExecuteReader();
        
        while (reader.Read())
        {
            if (reader.FieldCount == 0) 
                return false;
            
            var id = reader.GetString(0);
            var mmr = reader.GetInt32(1);
            user = new User(id, mmr);
            return true;
        }

        return false;
    }

    public User? GetUserByDiscordId(string discordId)
    {
        var command = _connection.CreateCommand();
        command.CommandText = $"SELECT * FROM UserData WHERE discordId = {discordId}";
        
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            if (reader.FieldCount == 0) 
                return null;
            
            var id = reader.GetString(0);
            var mmr = reader.GetInt32(1);
            var discord = reader.GetString(2);
            
            return new User(id, mmr, discord);
        }
        
        return null;
    }

    public bool AddNewUserToDatabase(string id, out User? newUser)
    {
        var query = _connection.CreateCommand();
        query.CommandText = $"SELECT COUNT(*) FROM UserData WHERE userData.id = {id}";
        if (query.ExecuteScalar()?.ToString() != "0")
        {
            TryGetUserById(id, out newUser);
            return false;
        }
        
        var addNewUserCommand = _connection.CreateCommand();
        addNewUserCommand.CommandText = $"INSERT INTO UserData (id, mmr) VALUES ({id}, 1000)";
        addNewUserCommand.ExecuteNonQuery();
        TryGetUserById(id, out newUser);
        return true;
    }

    protected override string DatabaseName => "LoungeData";
    
    protected override void CreateInitialTable()
    {
        var createDBCommand = _connection.CreateCommand();
        createDBCommand.CommandText = "CREATE TABLE IF NOT EXISTS UserData ( id TEXT NOT NULL PRIMARY KEY, mmr INTEGER NOT NULL, discordId TEXT );";
        createDBCommand.ExecuteNonQuery();
    }
}