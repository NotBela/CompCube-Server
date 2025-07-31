namespace LoungeSaber_Server.ServerMaintenanceState;

public static class ServerMaintenanceStateController
{
    private static ServerState _state = ServerState.Maintenance;

    public static ServerState State
    {
        get => _state;
        set
        {
            _state = value;
            OnStateChanged?.Invoke(value);
        }
    }
    
    public static event Action<ServerState>? OnStateChanged;
    
    public enum ServerState
    {
        Online,
        Maintenance
    }
}