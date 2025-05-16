using System.Text.Json.Serialization;

namespace LoungeSaber_Server.Models.Maps;

public class VotingMap
{
    [JsonPropertyName("hash")]
    public string Hash { get; set; }
    
    [JsonPropertyName("characteristic")]
    public string Characteristic { get; set; }
    
    [JsonPropertyName("difficulty")]
    public string Difficulty { get; set; }
}