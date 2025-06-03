using Newtonsoft.Json;

namespace LoungeSaber_Server.Models.Packets.UserPackets;

public class JoinPacket : UserPacket
{
    [JsonProperty("username")]
    public string UserName { get; private set; }
    
    [JsonProperty("userId")]
    public string UserId { get; private set; }
}