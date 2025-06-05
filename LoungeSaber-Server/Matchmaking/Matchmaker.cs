using LoungeSaber_Server.Models.Client;
using Timer = System.Timers.Timer;

namespace LoungeSaber_Server.Matchmaking;

public static class Matchmaker
{
    private static List<MatchmakingClient> _clientPool = [];

    private static Timer _mmrThresholdTimer = new Timer
    {
        Enabled = true,
        AutoReset = true,
        Interval = 5000
    };

    public static void AddClientToPool(ConnectedClient client)
    {
        _clientPool.Add(new MatchmakingClient(client));
    }
}