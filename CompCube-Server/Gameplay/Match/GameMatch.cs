using CompCube_Models.Models.ClientData;
using CompCube_Models.Models.Map;
using CompCube_Models.Models.Match;
using CompCube_Models.Models.Packets;
using CompCube_Models.Models.Packets.ServerPackets;
using CompCube_Models.Models.Packets.UserPackets;
using CompCube_Server.Discord.Events;
using CompCube_Server.Gameplay.Match.Dealer;
using CompCube_Server.Interfaces;
using CompCube_Server.Logging;
using CompCube_Server.SQL;

namespace CompCube_Server.Gameplay.Match;

public class GameMatch(MapData mapData, Logger logger, UserData userData, MatchLog matchLog, IDiscordBot messageManager, RankingData rankingData) : IDisposable
{
    private MatchSettings _matchSettings;

    private ClientManager _red;
    private ClientManager _blue;

    private bool _firstClientAlreadyFinishedDiscarding = false;

    private bool _lastPickWasRed = false;
    
    public void Init(MatchSettings matchSettings, IConnectedClient red, IConnectedClient blue)
    {
        _matchSettings = matchSettings;
        
        _red = new ClientManager(red, new DealerV0(mapData), true);
        _blue = new ClientManager(blue, new DealerV0(mapData), false);
    }

    public void StartMatch() => StartMatchAsync();

    public async Task StartMatchAsync()
    {
        _red.OnClientFinishedDiscarding += HandleClientFinishedDiscarding;
        _blue.OnClientFinishedDiscarding += HandleClientFinishedDiscarding;

        await _red.StartMatchForClient(_blue.ConnectedClient.UserInfo);
        await _blue.StartMatchForClient(_red.ConnectedClient.UserInfo);
    }

    private void HandleClientFinishedDiscarding(ClientManager client)
    {
        client.OnClientFinishedDiscarding -= HandleClientFinishedDiscarding;

        if (!_firstClientAlreadyFinishedDiscarding)
        {
            _firstClientAlreadyFinishedDiscarding = true;
            return;
        }
        
        StartPickPhase();
    }

    private void StartPickPhase()
    {
        
    }

    public void Dispose()
    {
        
    }

    private async Task SendToAllClients(ServerPacket serverPacket)
    {
        await _red.ConnectedClient.SendPacket(serverPacket);
        await _blue.ConnectedClient.SendPacket(serverPacket);
    }
}

public class ClientManager
{
    public readonly IConnectedClient ConnectedClient;

    private readonly IDealer _dealer;
    
    private List<VotingMap> _availablePicks;
    public IReadOnlyList<VotingMap> AvailablePicks => _availablePicks;
    
    public int Health { get; private set; } = 1000000;

    public readonly bool IsRed;

    public event Action<ClientManager>? OnHealthDidReachZero;

    public event Action<ClientManager>? OnClientFinishedDiscarding;
    
    public ClientManager(IConnectedClient client, IDealer dealer, bool isRed)
    {
        ConnectedClient = client;
        
        _dealer = dealer;
        
        _availablePicks = dealer.PullNewCards(5).ToList();
        
        IsRed = isRed;
    }

    public async Task StartMatchForClient(UserInfo opponent)
    {
        var red = IsRed ? ConnectedClient.UserInfo : opponent;
        var blue = !IsRed ? ConnectedClient.UserInfo : opponent;
        
        ConnectedClient.OnUserDiscardedMaps += HandleUserFinishedDiscarding;
        
        await ConnectedClient.SendPacket(new MatchCreatedPacket(red, blue, AvailablePicks.ToArray()));
    }

    private void HandleUserFinishedDiscarding(DiscardMapsPacket packet, IConnectedClient client)
    {
        client.OnUserDiscardedMaps -= HandleUserFinishedDiscarding;
        
        _dealer.AddDiscarded(packet.Maps);

        _availablePicks = _availablePicks.Concat(_dealer.PullNewCards(2)).Take(5).ToList();
        OnClientFinishedDiscarding?.Invoke(this);
    }

    public void Damage(int amount)
    {
        Health = Math.Max(Health - amount, 0);
        
        if (Health <= 0)
            OnHealthDidReachZero?.Invoke(this);
    }
}