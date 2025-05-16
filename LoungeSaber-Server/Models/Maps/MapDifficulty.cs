using System.Text.Json.Serialization;

namespace LoungeSaber_Server.Models.Maps;

public class MapDifficulty(string characteristic, string difficulty)
{
    [JsonPropertyName("characteristic")]
    public string Characteristic { get; set; } = characteristic;

    [JsonPropertyName("name")]
    public string Difficulty { get; set; } = difficulty;
}