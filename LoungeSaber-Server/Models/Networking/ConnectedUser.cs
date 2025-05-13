using System.Net.Sockets;
using System.Text;

namespace LoungeSaber_Server.Models.Networking;

public class ConnectedUser
{
    public readonly User UserInfo;
    private readonly TcpClient _client;

    public ConnectedUser(User user, TcpClient client)
    {
        UserInfo = user;
        _client = client;
    }

    public async Task SendServerAction(ServerAction action) =>
        await _client.GetStream().WriteAsync(Encoding.UTF8.GetBytes(action.Serialize()));
}