using CompCube_Models.Models.Map;
using CompCube_Server.Logging;

namespace CompCube_Server.Data;

public class MapData
{
    private readonly Logger _logger;
    private readonly DbSession _dbSession;

    public MapData(Logger logger, DbSession dbSession)
    {
        _logger = logger;
        _dbSession = dbSession;
        
        CreateInitialTables();
    }

    private void CreateInitialTables()
    {
        using var connection = _dbSession.CreateNewConnection();
        var createDbCommand = connection.CreateCommand();
        createDbCommand.CommandText = "CREATE TABLE IF NOT EXISTS mapData ( hash TEXT NOT NULL, difficulty TEXT NOT NULL, category TEXT NOT NULL, active BOOLEAN NOT NULL);";
        createDbCommand.ExecuteNonQuery();
    }

    public void AddMap(VotingMap votingMap)
    {
        using var connection = _dbSession.CreateNewConnection();
        var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO mapData VALUES (@hash, @difficulty, @category, true)";
        command.Parameters.AddWithValue("hash", votingMap.Hash);
        command.Parameters.AddWithValue("difficulty", votingMap.Difficulty.ToString());
        command.Parameters.AddWithValue("category", votingMap.MapCategory.ToString());

        command.ExecuteNonQuery();
    }

    public void DisableMap(VotingMap votingMap)
    {
        using var connection = _dbSession.CreateNewConnection();
        var command = connection.CreateCommand();
        
        command.CommandText = "UPDATE mapData SET active = false WHERE hash = @hash AND difficulty = @difficulty;";
        command.Parameters.AddWithValue("hash", votingMap.Hash);
        command.Parameters.AddWithValue("difficulty", votingMap.Difficulty.ToString());

        command.ExecuteNonQuery();
    }

    public List<VotingMap> GetAllMaps(List<VotingMap> exclude = null!)
    {
        var maps = new List<VotingMap>();
        
        using var connection = _dbSession.CreateNewConnection();
        var dbCommand = connection.CreateCommand();
        dbCommand.CommandText = "SELECT * FROM mapData WHERE active = true;";
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
            
            maps.Add(new VotingMap(hash, difficulty, category));
        }

        if(exclude != null) maps = maps.Where(m => !exclude.Any(e => e.Hash == m.Hash)).ToList();

        return maps;
    }
}