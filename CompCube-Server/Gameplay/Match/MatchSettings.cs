namespace CompCube_Server.Gameplay.Match;

public class MatchSettings(bool logMatch, bool competitive, int kFactor)
{
    public readonly bool LogMatch = logMatch;
    
    public readonly bool Competitive = competitive;
    
    public readonly int KFactor = kFactor;
}