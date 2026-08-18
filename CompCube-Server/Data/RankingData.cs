using CompCube_Models.Models.ClientData;
using CompCube_Models.Models.Match;
using CompCube_Server.Config;
using CompCube_Server.Logging;

namespace CompCube_Server.Data;

public class RankingData
{
    private readonly IConfiguration _config;
    private readonly DbSession _dbSession;
    private readonly RankFetcher _rankFetcher;
    private readonly ConfigHelper _configHelper;

    public RankingData(IConfiguration config, DbSession dbSession, RankFetcher rankFetcher, ConfigHelper configHelper)
    {
        _config = config;
        _dbSession = dbSession;
        _rankFetcher = rankFetcher;
        _configHelper = configHelper;

        CreateInitialTables();
    }

    private void CreateInitialTables()
    {
        using var connection = _dbSession.CreateNewConnection();
        var command = connection.CreateCommand();

        command.CommandText = "CREATE TABLE IF NOT EXISTS rankingData (season INT NOT NULL, id SERIAL NOT NULL, mmr INT NOT NULL, wins INT NOT NULL DEFAULT 0, totalGames INT NOT NULL DEFAULT 0, winstreak INT NOT NULL DEFAULT 0, bestWinstreak INT NOT NULL DEFAULT 0)";
        command.ExecuteNonQuery();
    }

    public void IncrementWins(UserInfo user)
    {
        using var connection = _dbSession.CreateNewConnection();
        using var incrementWinsCommand = connection.CreateCommand();
        incrementWinsCommand.CommandText = "UPDATE rankingData SET wins = wins + 1 WHERE id = @id AND season = @season LIMIT 1";
        incrementWinsCommand.Parameters.AddWithValue("@id", ulong.Parse(user.UserId));
        incrementWinsCommand.Parameters.AddWithValue("@season", _configHelper.Season);
        incrementWinsCommand.ExecuteNonQuery();

        using var incrementWinstreakCommand = connection.CreateCommand();
        incrementWinstreakCommand.CommandText = "UPDATE rankingData SET winstreak = winstreak + 1 WHERE id = @id AND season = @season LIMIT 1";
        incrementWinstreakCommand.Parameters.AddWithValue("@id", ulong.Parse(user.UserId));
        incrementWinstreakCommand.Parameters.AddWithValue("@season", _configHelper.Season);
        incrementWinstreakCommand.ExecuteNonQuery();

        if (user.Winstreak + 1 < user.HighestWinstreak)
            return;

        using var incrementBestWinstreakCommand = connection.CreateCommand();
        incrementBestWinstreakCommand.CommandText = "UPDATE rankingData SET bestWinstreak = winstreak WHERE id = @id AND season = @season LIMIT 1";
        incrementBestWinstreakCommand.Parameters.AddWithValue("@id", ulong.Parse(user.UserId));
        incrementBestWinstreakCommand.Parameters.AddWithValue("@season", _configHelper.Season);
        incrementBestWinstreakCommand.ExecuteNonQuery();
    }

    public void ResetWinstreak(UserInfo user)
    {
        using var connection = _dbSession.CreateNewConnection();
        using var resetWinstreakCommand = connection.CreateCommand();
        resetWinstreakCommand.CommandText = "UPDATE rankingData SET winstreak = 0 WHERE id = @id AND season = @season LIMIT 1";
        resetWinstreakCommand.Parameters.AddWithValue("@id", ulong.Parse(user.UserId));
        resetWinstreakCommand.Parameters.AddWithValue("@season", _configHelper.Season);
        resetWinstreakCommand.ExecuteNonQuery();
    }

    public void IncrementTotalGames(UserInfo user)
    {
        using var connection = _dbSession.CreateNewConnection();
        using var incrementTotalGamesCommand = connection.CreateCommand();
        incrementTotalGamesCommand.CommandText = "UPDATE rankingData SET totalGames = totalGames + 1 WHERE id = @id AND season = @season LIMIT 1";
        incrementTotalGamesCommand.Parameters.AddWithValue("@id", ulong.Parse(user.UserId));
        incrementTotalGamesCommand.Parameters.AddWithValue("@season", _configHelper.Season);
        incrementTotalGamesCommand.ExecuteNonQuery();
    }
    
    public void AdjustMmr(string userId, int change)
    {
        using var connection = _dbSession.CreateNewConnection();
        var command = connection.CreateCommand();
        command.CommandText = "UPDATE rankingData SET mmr = mmr + @change WHERE rankingData.id = @id AND season = @season LIMIT 1";
        command.Parameters.AddWithValue("season", _configHelper.Season);
        command.Parameters.AddWithValue("change", change);
        command.Parameters.AddWithValue("id", ulong.Parse(userId));
        command.ExecuteNonQuery();
    }
    
    public void CreateRankingDataForUserIfNotExists(string userId)
    {
        using var connection = _dbSession.CreateNewConnection();
        using var indexCommand = connection.CreateCommand();
        
        indexCommand.CommandText = "SELECT COUNT(*) FROM rankingData WHERE id = @userId AND season = @season";
        indexCommand.Parameters.AddWithValue("@userId", ulong.Parse(userId));
        indexCommand.Parameters.AddWithValue("@season", _configHelper.Season);
        var result = (long) indexCommand.ExecuteScalar()!;

        if (result >= 1)
            return;

        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO rankingData VALUES (@season, @id, 1000, 0, 0, 0, 0)";
        command.Parameters.AddWithValue("@id", ulong.Parse(userId));
        command.Parameters.AddWithValue("@season", _configHelper.Season);
        command.ExecuteNonQuery();
    }

    public RankData GetRankingData(string userId)
    {
        using var connection = _dbSession.CreateNewConnection();
        var command = connection.CreateCommand();
        
        command.CommandText = "SELECT * FROM rankingData WHERE id = @id AND season = @season";
        command.Parameters.AddWithValue("@id", ulong.Parse(userId));
        command.Parameters.AddWithValue("@season", _configHelper.Season);

        using var reader = command.ExecuteReader();

        var elo = -1;
        var wins = -1;
        var totalGames = -1;
        var winstreak = -1;
        var bestWinstreak = -1;

        while (reader.Read())
        {
            elo = reader.GetInt32(2);
            wins = reader.GetInt32(3);
            totalGames = reader.GetInt32(4);
            winstreak = reader.GetInt32(5);
            bestWinstreak = reader.GetInt32(6);
        }
        
        var rank = _rankFetcher.GetRankFromElo(elo);

        return new((int) rank, elo, wins, totalGames, winstreak, bestWinstreak);
    }
    
    
}