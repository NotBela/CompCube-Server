using LoungeSaber_Server.SQL;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LoungeSaber_Server.Models.Networking;

public class UserAction
{
    public readonly ActionType Type;
    public readonly JObject JsonData;

    [JsonIgnore]
    public User User
    {
        get
        {
            if (!JsonData.TryGetValue("userId", out var userId) || !UserData.Instance.TryGetUserById(userId.ToObject<string>()!, out var user) || user == null)
                throw new Exception("Could not find user!");

            return user;
        }
    }
    
    private UserAction(ActionType actionType, string data)
    {
        Type = actionType;

        JsonData = JsonConvert.DeserializeObject<JObject>(data)!;
    }

    public static UserAction Parse(string json)
    {
        var deserialized = JsonConvert.DeserializeObject<UserAction>(json);
        
        if (deserialized == null) 
            throw new Exception("User action could not be deserialized");
        
        return deserialized;
    }
    
    public enum ActionType
    {
        VoteOnMap,
        PostScore,
        Join,
        Leave
    }
}