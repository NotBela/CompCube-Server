using NetCord.Rest;

namespace CompCube_Server.Data;

public class RankFetcher(DbSession db, IConfiguration configuration)
{
    private int CurrentSeason => configuration.GetSection("Server").GetValue("Season", 0);
    
    public long GetRankFromElo(int elo)
    {
        using var command = db.Connection.CreateCommand();
        
        command.CommandText = "SELECT COUNT(*) FROM userData JOIN rankingData USING (id) WHERE mmr > @mmrThreshold AND banned = false AND season = @season ORDER BY mmr";
        command.Parameters.AddWithValue("@season", CurrentSeason);
        command.Parameters.AddWithValue("@mmrThreshold", elo);
        return (long) (command.ExecuteScalar() ?? -1) + 1;
    }
}