using System.Text;
using Newtonsoft.Json;

namespace LoungeSaber_Server.Models.Packets;

public abstract class Packet
{
    public byte[] SerializeToBytes() => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this));
}