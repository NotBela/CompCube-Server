using CompCube_Models.Models.Map;
using CompCube_Server.Logging;

namespace CompCube_Server.Data;

public class MapData
{
    private readonly Logger _logger;
    private readonly DbSession _dbSession;
    private readonly IConfiguration _config;

    public MapData(Logger logger, DbSession dbSession, IConfiguration config)
    {
        _logger = logger;
        _dbSession = dbSession;
        _config = config;

        CreateInitialTables();
    }

    private void CreateInitialTables()
    {
        using var connection = _dbSession.CreateNewConnection();
        var createDbCommand = connection.CreateCommand();
        createDbCommand.CommandText = "CREATE TABLE IF NOT EXISTS mapData ( hash TEXT NOT NULL, difficulty TEXT NOT NULL, category TEXT NOT NULL, batch TINYINT NOT NULL);";
        createDbCommand.ExecuteNonQuery();
    }

    public void AddMap(VotingMap votingMap, int batch)
    {
        using var connection = _dbSession.CreateNewConnection();
        var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO mapData VALUES (@hash, @difficulty, @category, @batch)";
        command.Parameters.AddWithValue("hash", votingMap.Hash);
        command.Parameters.AddWithValue("difficulty", votingMap.Difficulty.ToString());
        command.Parameters.AddWithValue("category", votingMap.MapCategory.ToString());
        command.Parameters.AddWithValue("batch", batch);

        command.ExecuteNonQuery();
    }

    public List<VotingMap> GetAllMaps(int[]? batches = null)
    {
        batches ??= _config.GetSection("Maps").GetValue<int[]>("ActiveBatches", [0]);
        
        var maps = new List<VotingMap>();
        
        using var connection = _dbSession.CreateNewConnection();
        var dbCommand = connection.CreateCommand();
        dbCommand.CommandText = "SELECT * FROM mapData;";
        using var reader = dbCommand.ExecuteReader();

        while (reader.Read())
        {
            if (reader.FieldCount == 0) return [];
            
            var hash = reader.GetString(0);

            if (!Enum.TryParse<VotingMap.DifficultyType>(reader.GetString(1), out var difficulty))
            {
                _logger.Error($"Could not parse difficulty type for hash {hash}: {reader.GetString(1)}");
                continue;
            }
            
            var categoryString = reader.GetString(2);

            if (!Enum.TryParse<VotingMap.Category>(categoryString, out var category))
            {
                _logger.Error($"Could not parse category for hash {hash}: {categoryString}");
                continue;
            }

            var batch = reader.GetInt32(3);

            if (!batches.Contains(batch))
                continue;

            maps.Add(new VotingMap(hash, difficulty, category));
        }

        return maps;
    }
}