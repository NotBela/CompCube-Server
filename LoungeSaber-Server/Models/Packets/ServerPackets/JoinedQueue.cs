namespace LoungeSaber_Server.Models.Packets.ServerPackets;

public class JoinedQueue : ServerPacket
{
    public override ServerPacket.ServerPacketTypes PacketType => ServerPacketTypes.JoinedQueue;
}