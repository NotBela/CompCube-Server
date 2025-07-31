using Newtonsoft.Json;

namespace LoungeSaber_Server.Models.Packets.ServerPackets;

public class JoinResponsePacket : ServerPacket
{
    public override ServerPacketTypes PacketType => ServerPacketTypes.JoinResponse;
        
    [JsonProperty("successful")]
    public bool Successful { get; private set; }
        
    [JsonProperty("message")]
    public string Message { get; private set; }

    public JoinResponsePacket(bool successful, string message)
    {
        Successful = successful;
        Message = message;
    }
}