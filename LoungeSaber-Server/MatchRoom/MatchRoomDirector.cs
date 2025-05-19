using System.Net;
using System.Net.Sockets;
using System.Text;
using LoungeSaber_Server.MatchRoom;
using LoungeSaber_Server.SkillDivision;
using LoungeSaber_Server.SQL;

namespace LoungeSaber_Server.Models.Networking;

public static class MatchRoomDirector
{
    private static readonly TcpListener Listener = new(IPAddress.Parse("127.0.0.1"), 8008);

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
                var client = Listener.AcceptTcpClient();
            
                try
                {
                    var buffer = new byte[1024];

                    var streamLength = client.GetStream().Read(buffer, 0, buffer.Length);
                    buffer = buffer[..streamLength];
                
                    var roomRequest = UserAction.Parse(Encoding.UTF8.GetString(buffer));

                    if (roomRequest.Type != UserAction.ActionType.Join ||
                        !roomRequest.JsonData.TryGetValue("divisionName", out var divisionName) ||
                        !DivisionManager.TryGetDivisionFromName(divisionName.ToObject<string>()!, out var division) || 
                        !roomRequest.JsonData.TryGetValue("userId", out var userId))
                    {
                        client.Close();
                        continue;
                    }

                    if (!UserData.Instance.TryGetUserById(userId.ToObject<string>()!, out var user))
                        UserData.Instance.AddNewUserToDatabase(userId.ToObject<string>()!, out user);

                    if (user == null)
                    {
                        client.Close();
                        continue;
                    }

                    if (!await division?.DivisionLobby.JoinRoom(new ConnectedUser(user, client))!)
                        continue;
                
                    client.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    client.Close();
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public static void Stop()
    {
        IsStarted = false;
        Listener.Stop();
    }
}