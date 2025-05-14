using System.Globalization;
using LoungeSaber_Server.Models;
using LoungeSaber_Server.Models.Networking;
using LoungeSaber_Server.SkillDivision;
using Newtonsoft.Json.Linq;

namespace LoungeSaber_Server.MatchRoom;

public class MatchRoom
{
    public readonly Division Division;
    
    public List<ConnectedUser> ConnectedUsers = [];

    public bool MatchInProgress { get; private set; } = false;
    
    public MatchRoom(Division division)
    {
        Division = division;
    }
    
    public bool CanJoinRoom(int mmr) => mmr >= Division.MinMMR && (Division.MaxMMR == 0 || mmr <= Division.MaxMMR);

    public bool JoinRoom(ConnectedUser user)
    {
        if (!CanJoinRoom(user.UserInfo.MMR)) return false;
        
        ConnectedUsers.Add(user);
        return true;
    }
    
    public bool CanStartMatch => ConnectedUsers.Count % 2 == 0 && ConnectedUsers.Count > 0 && !MatchInProgress;

    public async Task StartMatch()
    {
        if (MatchInProgress) return;

        var usersInMatch = ConnectedUsers.ToArray();

        var startingTime = DateTime.UtcNow.Add(new TimeSpan(0, 0, 0, 5));

        var startingMatchServerAction = new ServerAction(ServerAction.ActionType.StartWarning, new JObject()
        {
            {"startTime", startingTime.ToString(CultureInfo.InvariantCulture)}
        });

        await SendToClients(startingMatchServerAction, usersInMatch);
        
        await Task.Delay((int) startingTime.Subtract(DateTime.UtcNow).TotalMilliseconds);

        var mapVotingOptions = Division.GetRandomMaps(3);

        var mapVotingServerAction = new ServerAction(ServerAction.ActionType.ProvideVotes, new JObject()
        {
            {"mapVotingOptions", JArray.FromObject(mapVotingOptions)}
        });
    }

    public async Task SendToClients(ServerAction action, ConnectedUser[] users)
    {
        foreach (var client in ConnectedUsers)
        {
            await client.SendServerAction(action);
        }
    }
}