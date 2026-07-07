using CompCube_Server.Interfaces;

namespace CompCube_Server.Gameplay.Match;

public class GameMatchFactory(IServiceProvider services)
{
    public GameMatch CreateNewMatch(IConnectedClient red, IConnectedClient blue, MatchSettings settings)
    {
        var match = ActivatorUtilities.CreateInstance<GameMatch>(services);
        
        match.Init(settings, red, blue);

        return match;
    }
}