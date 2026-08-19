using CompCube_Server.Interfaces;

namespace CompCube_Server.Gameplay.Matchmaking;

public class QueueManager
{
    private readonly IQueue[] _staticQueues;
    
    public QueueManager(IEnumerable<IQueue> staticQueues, ILogger<QueueManager> logger)
    {
        _staticQueues = staticQueues.ToArray();
        
        logger.LogInformation("Initialized with {queueLength} queue(s)", _staticQueues.Length);
    }

    public IQueue? GetQueueFromName(string name)
    {
        
        return _staticQueues.FirstOrDefault(i => i.QueueName == name);
    }
}