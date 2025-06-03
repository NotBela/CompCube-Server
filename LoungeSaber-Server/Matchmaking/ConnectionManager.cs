using System.Net;
using System.Net.Sockets;
using System.Text;
using LoungeSaber_Server.Models.Packets;
using LoungeSaber_Server.Models.Packets.UserPackets;

namespace LoungeSaber_Server.Matchmaking;

public static class ConnectionManager
{
    private static TcpListener _listener = new(IPAddress.Loopback, 8008);
    
    private static Thread _listenerThread;

    private static bool _shouldListen = false;

    static ConnectionManager()
    {
        _listenerThread = new Thread(ListenForClients);
    }
    
    public static void Start()
    {
        if (_shouldListen) 
            return;
        
        _listenerThread = new Thread(ListenForClients);
        
        _listener.Start();
        
        _shouldListen = true;
        _listenerThread.Start();
    }

    private static async void ListenForClients()
    {
        try
        {
            while (_shouldListen)
            {
                var client = await _listener.AcceptTcpClientAsync();
                
                try
                {
                    var stream = client.GetStream();

                    var buffer = new byte[client.ReceiveBufferSize];

                    var bufferSize = await stream.ReadAsync(buffer);
                    Array.Resize(ref buffer, bufferSize);

                    var joinPacket = UserPacket.Deserialize(Encoding.UTF8.GetString(buffer)) as JoinPacket;

                    var connectedClient = joinPacket!.UserId;
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    client.Dispose();
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}