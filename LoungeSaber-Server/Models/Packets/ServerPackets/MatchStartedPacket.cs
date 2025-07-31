using LoungeSaber_Server.Models.Map;
using Newtonsoft.Json;

namespace LoungeSaber_Server.Models.Packets.ServerPackets;

public class MatchStartedPacket : ServerPacket
{
    public override ServerPacketTypes PacketType => ServerPacketTypes.MatchStarted;

    [JsonProperty("map")]
    public readonly VotingMap MapSelected;

    [JsonProperty("transitionToGameTime")]
    public readonly DateTime TransitionToGameTime;
    
    [JsonProperty("startingTime")]
    public readonly DateTime StartingTime;

    [JsonConstructor]
    public MatchStartedPacket(VotingMap mapSelected, DateTime transitionToGameTime, DateTime startingTime)
    {
        MapSelected = mapSelected;
        TransitionToGameTime = transitionToGameTime;
        StartingTime = startingTime;
    }
}