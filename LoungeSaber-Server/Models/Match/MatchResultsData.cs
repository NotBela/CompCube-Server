using LoungeSaber_Server.Models.Map;

namespace LoungeSaber_Server.Models.Match;

public record MatchResultsData(MatchScore Winner, MatchScore Loser, int MmrChange, VotingMap? Map, bool Premature, int Id, DateTime Time);