using LoungeSaber_Server.Interfaces;
using LoungeSaber_Server.Models.Events;
using LoungeSaber_Server.Models.Match;
using LoungeSaber_Server.Models.Packets;
using LoungeSaber_Server.Models.Packets.ServerPackets.Event;

namespace LoungeSaber_Server.Gameplay.Events;

public class Event(EventData eventData) : IQueue
{
    public string QueueName => eventData.EventName;
    
    public EventData EventData => eventData;
    
    public event Action<MatchResultsData, Match.Match>? QueueMatchEnded;

    private readonly List<IConnectedClient> _connectedClients = [];
    
    public int ClientCount => _connectedClients.Count;
    
    public void AddClientToPool(IConnectedClient client)
    {
        _connectedClients.Add(client);

        client.OnDisconnected -= OnDisconnected;
    }

    void OnDisconnected(IConnectedClient c)
    {
        c.OnDisconnected -= OnDisconnected;
            
        _connectedClients.Remove(c);
    }
    
    public void StartEvent()
    {
        SendPacketToAllClients(new EventStartedPacket());
    }

    public void CreateTournamentBracket()
    {
        throw new NotImplementedException();
    }

    private void SendPacketToAllClients(ServerPacket packet)
    {
        _connectedClients.ForEach(i => i.SendPacket(packet));
    }
}