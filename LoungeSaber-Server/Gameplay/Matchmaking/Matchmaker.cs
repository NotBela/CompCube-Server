using System.Timers;
using LoungeSaber_Server.Models.Client;
using LoungeSaber_Server.Models.Packets.ServerPackets;
using Timer = System.Timers.Timer;

namespace LoungeSaber_Server.Gameplay.Matchmaking;

public static class Matchmaker
{
    private static List<MatchmakingClient> _clientPool = [];
    
    public static List<Match.Match> ActiveMatches = [];

    public static event Action<Match.Match>? OnMatchStarted;

    private static Timer _mmrThresholdTimer = new Timer
    {
        Enabled = true,
        AutoReset = true,
        Interval = 5000
    };

    static Matchmaker()
    {
        _mmrThresholdTimer.Elapsed += MatchmakingTimerElapsed;
    }

    private static void MatchmakingTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        if (_clientPool.Count < 2) 
            return;
        
        var match = new Match.Match(_clientPool[0].Client, _clientPool[1].Client);
        ActiveMatches.Add(match);
        OnMatchStarted?.Invoke(match);
        
        match.OnMatchEnded += OnMatchEnded;
        
        Task.Run(async () =>
        {
            await match.StartMatch();
        });
    }

    private static void OnMatchEnded(MatchResults? results, Match.Match match)
    {
        match.OnMatchEnded -= OnMatchEnded;

        _clientPool.Remove(GetMatchmakingClientFromConnectedClient(match.PlayerOne));
        _clientPool.Remove(GetMatchmakingClientFromConnectedClient(match.PlayerTwo));

        return;

        MatchmakingClient GetMatchmakingClientFromConnectedClient(ConnectedClient c) =>
            _clientPool.First(i => i.Client == c);
    }

    public static async Task AddClientToPool(ConnectedClient client)
    {
        await Task.Delay(100);

        if (Program.Debug)
        {
            var match = new Match.Match(client, new DummyConnectedClient());
            await match.StartMatch();
            return;
        }
        
        _clientPool.Add(new MatchmakingClient(client));
    }
}