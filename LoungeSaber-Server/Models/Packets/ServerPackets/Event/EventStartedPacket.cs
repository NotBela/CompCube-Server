namespace LoungeSaber_Server.Models.Packets.ServerPackets.Event;

public class EventStartedPacket : ServerPacket
{
    public override ServerPacketTypes PacketType => ServerPacketTypes.EventStarted;
}