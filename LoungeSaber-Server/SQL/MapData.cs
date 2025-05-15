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

    public List<Map> GetAllMaps()
    {
        var getMapsQuery = _connection.CreateCommand();
        getMapsQuery.CommandText = "SELECT * FROM mapData";
        using var reader = getMapsQuery.ExecuteReader();

        var mapList = new List<Map>();
        
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
            
            if (mapList.Any(i => i.hash == hash))
            {
                mapList.FirstOrDefault(i => i.hash == hash)!.difficulties.Add(new MapDifficulty(characteristic, difficulty, mapCategory));
                continue;
            }
            
            mapList.Add(new Map([new MapDifficulty(characteristic, difficulty, mapCategory)], hash));
        }

        return mapList;
    }

    public List<Map> GetMapsOfCategory(MapDifficulty.MapTypes mapCategory)
    {
        var maps = GetAllMaps();

        var returnMaps = new List<Map>();

        foreach (var map in maps)
        {
            map.difficulties = map.difficulties.Where(i => i.Category == mapCategory).ToList();
            if (map.difficulties.Count > 0)
                returnMaps.Add(map);
        }

        return returnMaps;
    }
}