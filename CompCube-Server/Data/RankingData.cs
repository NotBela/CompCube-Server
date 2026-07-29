using CompCube_Models.Models.ClientData;
using CompCube_Models.Models.Match;
using CompCube_Server.Logging;

namespace CompCube_Server.SQL;

public class RankingData(IConfiguration config, Logger logger, IConfiguration configuration) : TableManager(configuration)
{
    public int CurrentSeason => config.GetSection("Server").GetValue("Season", 0);
    
    protected override void CreateInitialTables()
    {
        var command = Connection.CreateCommand();

        command.CommandText = "CREATE TABLE IF NOT EXISTS rankingData (season INT NOT NULL, id TEXT NOT NULL, mmr INT NOT NULL, wins INT NOT NULL DEFAULT 0, totalGames INT NOT NULL DEFAULT 0, winstreak INT NOT NULL DEFAULT 0, bestWinstreak INT NOT NULL DEFAULT 0)";
        command.ExecuteNonQuery();
    }
    
    
    
    public void CreateRankingDataForUserIfNotExists(int userId)
    {
        using var indexCommand = Connection.CreateCommand();
        
        indexCommand.CommandText = "SELECT COUNT(*) FROM rankingData WHERE id = @userId AND season = @season";
        indexCommand.Parameters.AddWithValue("@userId", userId);
        indexCommand.Parameters.AddWithValue("@season", CurrentSeason);
        var result = (long) indexCommand.ExecuteScalar();

        if (result >= 1)
            return;

        using var command = Connection.CreateCommand();
        command.CommandText = "INSERT INTO rankingData VALUES (@season, @id, 1000, 0, 0, 0, 0)";
        command.Parameters.AddWithValue("@id", userId);
        command.Parameters.AddWithValue("@season", CurrentSeason);
        command.ExecuteNonQuery();
    }
}