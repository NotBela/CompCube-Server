using Newtonsoft.Json;

namespace LoungeSaber_Server.Models.Match;

public record Score(int Points, float RelativeScore, bool ProMode, int Misses, bool FullCombo)
{
    public static Score Empty => new Score(0, 0f, false, 0, true);
    
    public string Serialize() => JsonConvert.SerializeObject(this);

    public static Score? Deserialize(string? json)
    {
        return json == null ? null : JsonConvert.DeserializeObject<Score>(json);
    }
}