using Newtonsoft.Json;

namespace LoungeSaber_Server.Models.Networking;

public class ServerAction(ServerAction.ActionType actionType, string data)
{
    public string Data { get; set; } = data;
    public ActionType Type { get; set; } = actionType;

    public string Serialize() => JsonConvert.SerializeObject(this);

    public enum ActionType
    {
        StartMatch,
        ProvideVotes,
        EndVoting,
        PlayerJoined,
        PlayerLeft,
        StartWarning
    }
}