using System.Timers;
using LoungeSaber_Server.Models.Client;
using LoungeSaber_Server.Models.Match;
using LoungeSaber_Server.Models.Packets.ServerPackets;
using LoungeSaber_Server.SQL;
using Timer = System.Timers.Timer;

namespace LoungeSaber_Server.Gameplay.Matchmaking;

public class Matchmaker
{
    private readonly UserData _userData;
    private readonly MapData _mapData;
    private readonly MatchLog _matchLog;
    private readonly ConnectionManager _connectionManager;
    
    private readonly List<MatchmakingClient> _clientPool = [];
    
    public readonly List<Match.Match> ActiveMatches = [];

    public event Action<Match.Match>? OnMatchStarted;

    private readonly Timer _mmrThresholdTimer = new Timer
    {
        Enabled = true,
        AutoReset = true,
        Interval = 5000
    };

    public Matchmaker(UserData userData, MapData mapData, MatchLog matchLog, ConnectionManager connectionmanager)
    {
        _userData = userData;
        _mapData = mapData;
        _matchLog = matchLog;
        _connectionManager = connectionmanager;
        
        _mmrThresholdTimer.Elapsed += MatchmakingTimerElapsed;
        _connectionManager.OnClientJoined += OnClientJoined;
    }

    private async void OnClientJoined(ConnectedClient client)
    {
        try
        {
            await AddClientToPool(client);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private void MatchmakingTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        if (_clientPool.Count < 2) 
            return;
        
        var playerOne =  _clientPool[0];
        var playerTwo =  _clientPool[1];
        
        var match = new Match.Match(playerOne.Client, playerTwo.Client, _matchLog, _userData, _mapData);

        _clientPool.Remove(playerOne);
        _clientPool.Remove(playerTwo);
        
        ActiveMatches.Add(match);
        OnMatchStarted?.Invoke(match);
        
        match.OnMatchEnded += OnMatchEnded;
        
        Task.Run(async () =>
        {
            await match.StartMatch();
        });
    }

    private void OnMatchEnded(MatchResultsData results, Match.Match match)
    {
        match.OnMatchEnded -= OnMatchEnded;

        ActiveMatches.Remove(match);
    }

    private async Task AddClientToPool(ConnectedClient client)
    {
        await Task.Delay(100);

        if (Program.Debug)
        {
            var match = new Match.Match(client, new DummyConnectedClient(_userData.GetUserById("0") ?? throw new Exception()), _matchLog, _userData, _mapData);
            OnMatchStarted?.Invoke(match);
            await match.StartMatch();
            return;
        }
        
        _clientPool.Add(new MatchmakingClient(client));
    }
}