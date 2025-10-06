using LoungeSaber_Server.Interfaces;
using LoungeSaber_Server.Models.Match;

namespace LoungeSaber_Server.Gameplay.Events;

public class Event(EventData data) : IQueue
{
    public string QueueName => data.Name;
    
    public event Action<MatchResultsData, Match.Match>? QueueMatchEnded;

    private List<IConnectedClient> _connectedClients = [];
    
    public void AddClientToPool(IConnectedClient client)
    {
        _connectedClients.Add(client);
    }
}