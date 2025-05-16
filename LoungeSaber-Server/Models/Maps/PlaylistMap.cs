using System.Text.Json.Serialization;

namespace LoungeSaber_Server.Models.Maps;

public class PlaylistMap
{
    [JsonPropertyName("hash")]
    public string Hash { get; set; }
    
    [JsonPropertyName("difficulties")]
    public MapDifficulty[] Difficulties { get; set; }
}