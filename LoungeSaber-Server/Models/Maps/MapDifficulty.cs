using LoungeSaber_Server.Models.Maps;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LoungeSaber_Server.Models;

public class MapDifficulty
{
    [JsonProperty("characteristic")]
    public string Characteristic { get; private set; }
    
    // ??? why does playlistmanager serialize the difficulty like this
    [JsonProperty("name")]
    public string Difficulty { get; private set; }
    
    [JsonIgnore]
    public MapTypes Category { get; private set; }
    
    public MapDifficulty(string characteristic, string difficulty, MapTypes category)
    {
        Characteristic = characteristic;
        Difficulty = difficulty;
        Category = category;
    }

    public enum MapTypes
    {
        Acc,
        Speed,
        Tech,
        Midspeed,
        Balanced,
        Extreme,
        Unknown
    }
}