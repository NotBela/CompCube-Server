using Newtonsoft.Json;

namespace LoungeSaber_Server.Models.Map;

public class VotingMap
{
    [JsonProperty("hash")]
    public readonly string Hash;

    [JsonProperty("difficulty")]
    public readonly DifficultyType Difficulty;

    [JsonProperty("category")]
    public readonly CategoryType Category;

    public VotingMap(string hash, DifficultyType difficulty, CategoryType category)
    {
        Hash = hash;
        Difficulty = difficulty;
        Category = category;
    }

    public string Serialize() => JsonConvert.SerializeObject(this);

    public static VotingMap? Deserialize(string json) => JsonConvert.DeserializeObject<VotingMap?>(json);

    public enum CategoryType
    {
        Acc,
        MidSpeed,
        Tech,
        Balanced,
        Speed,
        Extreme
    }
    
    public enum DifficultyType
    {
        Easy,
        Normal,
        Hard,
        Expert,
        ExpertPlus
    }
}