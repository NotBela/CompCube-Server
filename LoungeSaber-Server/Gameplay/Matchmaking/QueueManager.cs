using LoungeSaber_Server.Interfaces;
using LoungeSaber_Server.Logging;
using LoungeSaber_Server.Models.Match;

namespace LoungeSaber_Server.Gameplay.Matchmaking;

public class QueueManager
{
    private readonly IQueue[] _staticQueues;
    
    public event Action<MatchResultsData, Match.Match>? OnAnyMatchEnded;
    
    public QueueManager(IEnumerable<IQueue> staticQueues, Logger logger)
    {
        _staticQueues = staticQueues.ToArray();
        
        logger.Info(_staticQueues.Length.ToString());
        
        foreach (var queue in _staticQueues)
            queue.QueueMatchEnded += OnQueueMatchEnded;
    }

    private void OnQueueMatchEnded(MatchResultsData data, Match.Match match)
    {
        OnAnyMatchEnded?.Invoke(data, match);
    }

    public IQueue? GetQueueFromName(string name)
    {
        return _staticQueues.FirstOrDefault(i => i.QueueName == name);
    }
}