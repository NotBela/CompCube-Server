using Newtonsoft.Json;

namespace LoungeSaber_Server.Models.Client;

public class UserInfo
{
    [JsonProperty("username")]
    public string Username { get; private set; }
    
    [JsonProperty("userId")]
    public string UserId { get; private set; }
    
    [JsonProperty("mmr")]
    public int Mmr { get; private set; }
    
    [JsonProperty("badge")]
    public Badge Badge { get; private set; }
    
    [JsonConstructor]
    public UserInfo(string username, string userId, int mmr, Badge badge)
    {
        Username = username;
        UserId = userId;
        Mmr = mmr;
        Badge = badge;
    }
}

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