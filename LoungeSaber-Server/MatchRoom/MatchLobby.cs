using System.Globalization;
using LoungeSaber_Server.Models;
using LoungeSaber_Server.Models.Networking;
using LoungeSaber_Server.SkillDivision;
using Newtonsoft.Json.Linq;

namespace LoungeSaber_Server.MatchRoom;

public class MatchLobby
{
    public readonly Division Division;
    
    public List<ConnectedUser> ConnectedUsers = [];

    public bool MatchesInProgress { get; private set; } = false;
    
    public MatchLobby(Division division)
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
    
    public bool CanStartMatch => ConnectedUsers.Count % 2 == 0 && ConnectedUsers.Count > 0 && !MatchesInProgress;

    public async Task StartMatches()
    {
        if (MatchesInProgress) return;

        var usersInDivision = ConnectedUsers.ToArray();

        var startingTime = DateTime.UtcNow.Add(new TimeSpan(0, 0, 0, 5));

        var startingMatchServerAction = new ServerAction(ServerAction.ActionType.StartWarning, new JObject()
        {
            {"startTime", startingTime.ToString("o", CultureInfo.InvariantCulture)}
        });

        await SendToClients(startingMatchServerAction, usersInDivision);
        
        await Task.Delay((int) startingTime.Subtract(DateTime.UtcNow).TotalMilliseconds);

        CreateMatches();
    }

    private Match[] CreateMatches()
    {
        var users = ConnectedUsers.ToList();
        var random = new Random();

        var matches = new List<Match>();
        
        while (users.Count > 0)
        {
            var randomUserOne = users[random.Next(0, users.Count + 1)];
            users.Remove(randomUserOne);

            var randomUserTwo = users[random.Next(0, users.Count + 1)];
            users.Remove(randomUserTwo);
            
            matches.Add(new Match(randomUserOne, randomUserTwo, Division));
        }

        return matches.ToArray();
    }

    private async Task SendToClients(ServerAction action, ConnectedUser[] users)
    {
        foreach (var client in ConnectedUsers)
            await client.SendServerAction(action);
    }
}