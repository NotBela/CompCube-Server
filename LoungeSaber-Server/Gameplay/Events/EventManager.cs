using LoungeSaber_Server.Logging;
using LoungeSaber_Server.Models.Match;

namespace LoungeSaber_Server.Gameplay.Events;

public class EventManager
{
    private readonly Logger _logger;
    
    public event Action<MatchResultsData, Match.Match>? EventMatchEnded;
    
    private List<Event> _events = [];
    
    public IReadOnlyList<Event> ActiveEvents => _events;
    
    public EventManager(Logger logger)
    {
        _logger = logger;
    }

    public void AddEvent(Event e)
    {
        e.QueueMatchEnded += OnEventMatchEnded;
        
        _events.Add(e);
        
        _logger.Info($"Event {e.QueueName} has been created.");
    }

    public void RemoveEvent(Event e)
    {
        e.QueueMatchEnded -= OnEventMatchEnded;
        
        _events.Remove(e);
        
        _logger.Info($"Event {e.QueueName} has been removed.");
    }

    private void OnEventMatchEnded(MatchResultsData data, Match.Match match) => EventMatchEnded?.Invoke(data, match);
    
}