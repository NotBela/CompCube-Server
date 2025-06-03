using LoungeSaber_Server.Models.Packets.UserPackets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LoungeSaber_Server.Models.Packets;

public abstract class UserPacket : Packet
{
    public static UserPacket Deserialize(string json)
    {
        var jobj = JObject.Parse(json);
        
        if (!jobj.TryGetValue("packetType", out var packetTypeJToken))
            throw new Exception("Could not deserialize packet!");
        
        if (!Enum.TryParse<UserPacketTypes>(packetTypeJToken.ToObject<string>(), out var userPacketType))
            throw new Exception("Could not deserialize packet type!");

        return userPacketType switch
        {
            UserPacketTypes.Join => JsonConvert.DeserializeObject<JoinPacket>(json)!,
            _ => throw new Exception("Could not get packet type!")
        };
    }

    public enum UserPacketTypes
    {
        Join
    }
}