using LoungeSaber_Server.Models.Packets.UserPackets;
using Newtonsoft.Json;

namespace LoungeSaber_Server.Models.Packets.ServerPackets;

public class MatchResults : ServerPacket
{
    public override ServerPacketTypes PacketType => ServerPacketTypes.MatchResults;
    
    [JsonProperty("opponentScore")]
    public readonly ScoreSubmissionPacket OpponentScore;
        
    [JsonProperty("yourScore")]
    public readonly ScoreSubmissionPacket YourScore;

    [JsonProperty("winner")]
    public readonly MatchWinner Winner;
    
    [JsonProperty("mmrChange")]
    public readonly int MMRChange;

    [JsonConstructor]
    public MatchResults(ScoreSubmissionPacket opponentScore, ScoreSubmissionPacket yourScore, MatchWinner winner, int mmrChange)
    {
        OpponentScore = opponentScore;
        YourScore = yourScore;
        Winner = winner;
        MMRChange = mmrChange;
    }

    public enum MatchWinner
    {
        You,
        Opponent
    }
}