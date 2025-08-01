using Newtonsoft.Json;

namespace LoungeSaber_Server.Models.Server;

public class ServerStatusManager
{
    private ServerState _state = ServerState.Maintenance;

    public ServerState State
    {
        get => _state;
        set
        {
            _state = value;
            OnStateChanged?.Invoke(value);
        }
    }
    
    public event Action<ServerState>? OnStateChanged;
    
    public enum ServerState
    {
        Online,
        Maintenance
    }
    
    public ServerStatus GetServerStatus() => new(["1.39.1", "1.40.8", "1.40.5"], ["1.0.0"], State);
}