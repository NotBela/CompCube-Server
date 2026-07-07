using CompCube_Models.Models.ClientData;
using CompCube_Models.Models.Map;
using CompCube_Models.Models.Match;
using CompCube_Models.Models.Packets;
using CompCube_Models.Models.Packets.ServerPackets;
using CompCube_Models.Models.Packets.UserPackets;
using CompCube_Server.Discord.Events;
using CompCube_Server.Interfaces;
using CompCube_Server.Logging;
using CompCube_Server.SQL;

namespace CompCube_Server.Gameplay.Match;

public class GameMatch(MapData mapData, Logger logger, UserData userData, MatchLog matchLog, IDiscordBot messageManager, RankingData rankingData) : IDisposable
{
    private MatchSettings _matchSettings;

    private IConnectedClient _red;
    private IConnectedClient _blue;
    
    public void Init(MatchSettings matchSettings, IConnectedClient red, IConnectedClient blue)
    {
        _matchSettings = matchSettings;
        
        _red = red;
        _blue = blue;
    }

    public void StartMatch()
    {
        
    }

    public void Dispose()
    {
        
    }
}