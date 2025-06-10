using LoungeSaber_Server.Models.Client;

namespace LoungeSaber_Server.Gameplay.Matchmaking;

public class DebugMatchmaker : Matchmaker
{
    public static readonly DebugMatchmaker Instance = new();

    public override void AddClientToPool(ConnectedClient client)
    {
        
    }
}