using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LoungeSaber_Server.Models.Networking;

public class ServerAction(ServerAction.ActionType actionType, JObject data)
{
    public JObject Data { get; set; } = data;
    public ActionType Type { get; set; } = actionType;

    public string Serialize() => JsonConvert.SerializeObject(this);

    public enum ActionType
    {
        StartMatch,
        OpponentVoted,
        CreateMatch,
        MatchEnded,
        StartWarning,
        Results,
        UpdateConnectedUserCount
    }
}