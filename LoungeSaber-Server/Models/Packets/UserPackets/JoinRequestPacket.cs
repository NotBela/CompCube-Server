using Newtonsoft.Json;

namespace LoungeSaber_Server.Models.Packets.UserPackets;

public class JoinRequestPacket : UserPacket
{
    [JsonProperty("username")]
    public string UserName { get; private set; }
    
    [JsonProperty("userId")]
    public string UserId { get; private set; }

    [JsonProperty("queue")]
    public string Queue { get; private set; }
    
    [JsonConstructor]
    public JoinRequestPacket(string userName, string userId, string queue)
    {
        UserName = userName;
        UserId = userId;
        Queue = queue;
    }

    public override UserPacketTypes PacketType =>  UserPacketTypes.JoinRequest;
}