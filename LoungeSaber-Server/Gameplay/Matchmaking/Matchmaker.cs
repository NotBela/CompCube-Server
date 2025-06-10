using LoungeSaber_Server.Models.Client;
using Timer = System.Timers.Timer;

namespace LoungeSaber_Server.Gameplay.Matchmaking;

public class Matchmaker
{
    public static readonly Matchmaker Instance = new();
    
    private List<MatchmakingClient> _clientPool = [];

    private Timer _mmrThresholdTimer = new Timer
    {
        Enabled = true,
        AutoReset = true,
        Interval = 5000
    };

    public virtual void AddClientToPool(ConnectedClient client)
    {
        _clientPool.Add(new MatchmakingClient(client));
    }
}