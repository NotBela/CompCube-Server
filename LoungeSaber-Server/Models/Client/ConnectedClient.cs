using System.Net.Sockets;

namespace LoungeSaber_Server.Models.Client;

public class ConnectedClient
{
    private readonly TcpClient _client;
    
    public readonly UserInfo UserInfo;

    public ConnectedClient(TcpClient client, UserInfo userInfo)
    {
        _client = client;
        UserInfo = userInfo;
    }
}