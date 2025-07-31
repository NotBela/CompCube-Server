using LoungeSaber_Server.Models.Client;
using LoungeSaber_Server.Models.Map;

namespace LoungeSaber_Server.Models.Match;

public record MatchResultsData(MatchScore Winner, MatchScore Loser, int MmrChange, VotingMap? Map, bool Premature);

public record MatchScore(UserInfo User, int Score, float RelativeScore, bool ProMode, int Misses, bool FC)
{
    public static MatchScore GetEmptyMatchScore(UserInfo user) => new MatchScore(user, 0, 0f, false, 0, true);
}