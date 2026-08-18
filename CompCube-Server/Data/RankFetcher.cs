using CompCube_Server.Config;
using NetCord.Rest;

namespace CompCube_Server.Data;

public class RankFetcher(DbSession db, ConfigHelper config)
{
    
    public long GetRankFromElo(int elo)
    {
        using var connection = db.CreateNewConnection();
        var command = connection.CreateCommand();
        
        command.CommandText = "SELECT COUNT(*) FROM userData JOIN rankingData USING (id) WHERE mmr > @mmrThreshold AND banned = false AND season = @season ORDER BY mmr";
        command.Parameters.AddWithValue("@season", config.Season);
        command.Parameters.AddWithValue("@mmrThreshold", elo);
        return (long) (command.ExecuteScalar() ?? -1) + 1;
    }
}