using CompCube_Server.Interfaces;
using CompCube_Server.Logging;

namespace CompCube_Server.Gameplay.Matchmaking;

public class QueueManager
{
    private readonly IQueue[] _staticQueues;
    
    public QueueManager(IEnumerable<IQueue> staticQueues, Logger logger)
    {
        _staticQueues = staticQueues.ToArray();
        
        logger.Info($"Initialized with {_staticQueues.Length} queue(s)");
    }

    public IQueue? GetQueueFromName(string name)
    {
        
        return _staticQueues.FirstOrDefault(i => i.QueueName == name);
    }
}