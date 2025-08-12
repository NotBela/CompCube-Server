using LoungeSaber_Server.Models.ClientData;

namespace LoungeSaber_Server.Models.Match;

public record MatchScore(UserInfo User, Score? Score);