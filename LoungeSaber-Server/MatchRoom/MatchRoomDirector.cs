using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using LoungeSaber_Server.Models.Networking;
using LoungeSaber_Server.SQL;
using Newtonsoft.Json.Linq;

namespace LoungeSaber_Server.MatchRoom;

public static class MatchRoomDirector
{
    //TODO: change listener ip when not developing
    private static readonly TcpListener Listener = new(IPAddress.Loopback, 8008);

    private static readonly Thread listenForClientsThread = new(ListenForClients);
    
    private static bool IsStarted = false;

    public static void Start()
    {
        Listener.Start();
        IsStarted = true;
        
        listenForClientsThread.Start();
    }

    private static async void ListenForClients()
    {
        try
        {
            while (IsStarted)
            {
                var client = await Listener.AcceptTcpClientAsync();
                
                try
                {
                    var buffer = new byte[1024];

                    var streamLength = client.GetStream().Read(buffer, 0, buffer.Length);
                    buffer = buffer[..streamLength];

                    var json = Encoding.UTF8.GetString(buffer);
                    Console.WriteLine(json);
                
                    var roomRequest = UserPacket.Parse(json);

                    if (roomRequest.Type != UserPacket.ActionType.Join ||
                        !roomRequest.JsonData.TryGetValue("divisionName", out var divisionName) ||
                        !DivisionManager.TryGetDivisionFromName(divisionName.ToObject<string>()!, out var division) || 
                        !roomRequest.JsonData.TryGetValue("userId", out var userId) || 
                        !roomRequest.JsonData.TryGetValue("userName", out var userName))
                    {
                        await DisconnectClient(client, "InvalidJoinPacket");
                        continue;
                    }

                    if (!UserData.Instance.TryGetUserById(userId.ToObject<string>()!, out var user))
                        UserData.Instance.AddNewUserToDatabase(userId.ToObject<string>()!, out user);

                    if (user == null)
                    {
                        await DisconnectClient(client, "CouldNotFindUser");
                        continue;
                    }

                    if (!await division?.DivisionLobby.JoinRoom(new ConnectedUser(user, client,
                            userName.ToObject<string>()!))!)
                    {
                        await DisconnectClient(client, "CannotJoinRoom");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    await DisconnectClient(client, "UnhandledServerException");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static async Task DisconnectClient(TcpClient client, string reason)
    {
        var packet = new ServerPacket(ServerPacket.ActionType.Disconnect, new JObject
        {
            {"reason", reason}
        });

        var bytes = Encoding.UTF8.GetBytes(packet.Serialize());
        
        await client.Client.SendAsync(new ArraySegment<byte>(bytes));
        client.Close();
    }

    public static void Stop()
    {
        IsStarted = false;
        Listener.Stop();
    }
}