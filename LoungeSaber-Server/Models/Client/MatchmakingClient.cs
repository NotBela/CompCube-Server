using System.Timers;
using Timer = System.Timers.Timer;

namespace LoungeSaber_Server.Models.Client;

public class MatchmakingClient
{
    public ConnectedClient Client { get; private set; }

    public int MmrThreshold { get; private set; } = 100;
    
    public MatchmakingClient(ConnectedClient client)
    {
        Client = client;
    }

    private void OnMmrThresholdClockElapsed(object? sender, ElapsedEventArgs e)
    {
        MmrThreshold += 500;
    }

    public bool CanMatchWithOtherClient(MatchmakingClient other)
    {
        return Math.Abs(other.MmrThreshold - MmrThreshold) <= MmrThreshold;
    }
}