using LoungeSaber_Server.Models;
using LoungeSaber_Server.Models.Maps;
using Microsoft.Data.Sqlite;

namespace LoungeSaber_Server.SQL;

public class MapData : Database
{
    public static readonly MapData Instance = new();
    
    protected override string DatabaseName => "MapData";
    
    protected override void CreateInitialTable()
    {
        var createDBCommand = _connection.CreateCommand();
        createDBCommand.CommandText = "CREATE TABLE IF NOT EXISTS mapData ( " + 
                                      "hash TEXT NOT NULL, " + 
                                      "difficulty TEXT NOT NULL, " + 
                                      "characteristic TEXT NOT NULL, " + 
                                      "category TEXT NOT NULL " +
                                      ");";
        createDBCommand.ExecuteNonQuery();
    }

    public List<MapDifficulty> GetAllMaps()
    {
        var getMapsQuery = _connection.CreateCommand();
        getMapsQuery.CommandText = "SELECT * FROM mapData";
        using var reader = getMapsQuery.ExecuteReader();

        var mapList = new List<MapDifficulty>();
        
        while (reader.Read())
        {
            var hash = reader.GetString(0);
            var difficulty = reader.GetString(1);
            var characteristic = reader.GetString(2);
            var category = reader.GetString(3);

            if (!Enum.TryParse<MapDifficulty.MapTypes>(category, out var mapCategory))
            {
                Console.WriteLine("Could not parse map category!");
                mapCategory = MapDifficulty.MapTypes.Unknown;
            }
            
            mapList.Add(new MapDifficulty(hash,difficulty, characteristic, mapCategory));
        }

        return mapList;
    }

    public List<MapDifficulty> GetMapsOfCategory(MapDifficulty.MapTypes mapCategory)
    {
        var maps = GetAllMaps();

        var returnMaps = new List<MapDifficulty>();

        foreach (var map in maps)
        {
            if (map.Category == mapCategory)
                returnMaps.Add(map);
        }

        return returnMaps;
    }

    public bool TryGetMapByHash(string hash, out MapDifficulty? map)
    {
        map = GetAllMaps().FirstOrDefault(i => i.Hash == hash);
        return map != null;
    }
}