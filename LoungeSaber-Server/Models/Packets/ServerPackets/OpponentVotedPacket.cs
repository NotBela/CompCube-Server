using Newtonsoft.Json;

namespace LoungeSaber_Server.Models.Packets.ServerPackets;

public class OpponentVotedPacket : ServerPacket
{
    public override ServerPacketTypes PacketType => ServerPacketTypes.OpponentVoted;
    
    [JsonProperty("vote")]
    public readonly int VoteIndex;

    [JsonConstructor]
    public OpponentVotedPacket(int voteIndex)
    {
        VoteIndex = voteIndex;
    }
}