namespace CompCube_Server.Gameplay.Matchmaking;

public class TimeoutManager
{
    private readonly Dictionary<string, DateTime> _timeouts = new();
    
    public IReadOnlyDictionary<string, DateTime> Timeouts => _timeouts;

    public void TimeoutUser(string id, TimeSpan time)
    {
        _timeouts.Add(id, DateTime.Now.Add(time));
    }

    public bool IsUserTimedOut(string id)
    {
        if (!_timeouts.TryGetValue(id, out var timeout))
            return false;

        return DateTime.Now <= timeout;
    }

    public TimeSpan GetRemainingTimeoutTime(string userId)
    {
        if (!IsUserTimedOut(userId))
            return TimeSpan.Zero;
        
        return _timeouts[userId] - DateTime.Now;
    }
}