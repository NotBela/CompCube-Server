using Newtonsoft.Json;

namespace LoungeSaber_Server.Models.Packets.ServerPackets;

public class PrematureMatchEndPacket : ServerPacket
{
    public override ServerPacketTypes PacketType => ServerPacketTypes.PrematureMatchEnd;
    
    [JsonProperty("reason")]
    public readonly string Reason;

    [JsonConstructor]
    public PrematureMatchEndPacket(string reason)
    {
        Reason = reason;
    }
}