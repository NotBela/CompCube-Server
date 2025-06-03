using Newtonsoft.Json;

namespace LoungeSaber_Server.Models.Badge;


public class Badge
{
    [JsonProperty("badgeName")]
    public string Name { get; private set; }
    
    [JsonProperty("badgeColor")]
    public string ColorCode { get; private set; }
    
    [JsonProperty("badgeBold")]
    public bool Bold { get; private set; }

    [JsonConstructor]
    public Badge(string name, string colorCode, bool bold)
    {
        Name = name;
        ColorCode = colorCode;
        Bold = bold;
    }
}