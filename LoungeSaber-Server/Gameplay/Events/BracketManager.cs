using LoungeSaber_Server.Interfaces;

namespace LoungeSaber_Server.Gameplay.Events;

public class BracketManager
{
    
    
    public BracketManager(List<IConnectedClient> clients)
    {
        var bracketOrder = clients.OrderBy(i => i.UserInfo.Mmr);
    }
}