using CompCube_Models.Models.ClientData;
using CompCube_Server.Data;
using CompCube_Server.Gameplay.Match;
using CompCube_Server.Interfaces;
using CompCube_Server.Networking.Client;

namespace CompCube_Server.Gameplay.Matchmaking;

public class DebugQueue(ILogger<DebugQueue> logger, GameMatchFactory gameMatchFactory, MapData mapData) : IQueue
{
    public string QueueName => "debug";

    public void AddClientToPool(IConnectedClient client)
    {
        logger.LogInformation("Started debug match with {userName}", client.UserInfo.Username);
        
        var match = gameMatchFactory.CreateNewMatch(client, new DummyConnectedClient(new UserInfo("debug", "0", 1000, null, 0, null, false, 0, 0, 0, 0), mapData), new MatchSettings(false, false, 0, 0));
        match.StartMatch();
    }
}