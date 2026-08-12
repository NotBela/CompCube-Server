using CompCube_Models.Models.Map;
using CompCube_Server.Logging;

namespace CompCube_Server.Data;

public class MapQueue(IConfiguration config, Logger logger, MapData mapData) : TableManager(config)
{
    protected override void CreateInitialTables()
    {
        var command = Connection.CreateCommand();
        command.CommandText = "CREATE TABLE IF NOT EXISTS queue ( hash TEXT NOT NULL, difficulty TEXT NOT NULL, category TEXT NOT NULL);";
        command.ExecuteNonQuery();
    }

    public void RemoveFromQueueAndAdd(VotingMap votingMap)
    {
        var command = Connection.CreateCommand();
        command.CommandText = "DELETE FROM queue WHERE hash = @hash AND difficulty = @difficulty;";
        command.Parameters.AddWithValue("difficulty", votingMap.Difficulty.ToString());
        command.Parameters.AddWithValue("hash", votingMap.Hash);
        
        command.ExecuteNonQuery();
        
        mapData.AddMap(votingMap);
    }

    public void AddToQueue(VotingMap votingMap)
    {
        var command = Connection.CreateCommand();
        command.CommandText = "INSERT INTO queue VALUES (@hash, @difficulty, @category);";
        
        command.Parameters.AddWithValue("hash", votingMap.Hash);
        command.Parameters.AddWithValue("difficulty", votingMap.Difficulty.ToString());
        command.Parameters.AddWithValue("category", votingMap.MapCategory.ToString());
        
        command.ExecuteNonQuery();
    }

    public List<VotingMap> GetMaps()
    {
        var maps = new List<VotingMap>();

        var command = Connection.CreateCommand();
        command.CommandText = "SELECT * FROM queue;";
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            if (reader.FieldCount == 0) return [];

            var hash = reader.GetString(0);
            var diffString = reader.GetString(1);
            var categoryString = reader.GetString(2);

            if (!Enum.TryParse<VotingMap.DifficultyType>(diffString, out var difficulty))
            {
                logger.Info($"Failed to parse difficulty from hash {hash}: {difficulty}");
                continue;
            }

            if (!Enum.TryParse<VotingMap.Category>(categoryString, out var category))
            {
                logger.Info($"Failed to parse category from hash {hash}: {category}");
                continue;
            }
            
            maps.Add(new VotingMap(hash, difficulty, category));
        }
        
        return maps;
    }
}