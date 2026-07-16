using System.Net.WebSockets;
using System.Text;
using CompCube_Models.Models.Packets;
using CompCube_Models.Models.Packets.ServerPackets;
using CompCube_Models.Models.Packets.UserPackets;
using CompCube_Server.Interfaces;
using CompCube_Server.Logging;
using CompCube_Server.Networking.Client;
using CompCube_Server.SQL;

namespace CompCube_Server.Gameplay.Matchmaking;

public class ConnectionManager
{
    private readonly UserData _userData;
    private readonly Logger _logger;
    private readonly QueueManager _queueManager;
    
    private readonly List<IConnectedClient> _connectedClients = [];
    
    public ConnectionManager(UserData userData, Logger logger, QueueManager queueManager)
    {
        _userData = userData;
        _logger = logger;
        _queueManager = queueManager;
        
        Start();
    }

    public void Start()
    {
        Task.Factory.StartNew(PollAllClients, TaskCreationOptions.LongRunning);
        
        _logger.Info("Started listening for clients");
    }

    public async Task HandleWebSocket(WebSocket socket, TaskCompletionSource socketFinishedTcs)
    {
        var buffer = new byte[1024];

        var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        buffer = buffer[..result.Count];

        if (result.MessageType == WebSocketMessageType.Close)
        {
            await socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            socketFinishedTcs.SetResult();
            return;
        }
        
        var json = Encoding.UTF8.GetString(buffer);

        var couldParsePacket = UserPacket.TryDeserialize(json, out var packet);

        if (!couldParsePacket)
        {
            _logger.Error("Could not parse packet from client.");
            await socket.CloseOutputAsync(WebSocketCloseStatus.InvalidPayloadData, "", CancellationToken.None);
            socketFinishedTcs.SetResult();
            return;
        }

        if (packet is not JoinRequestPacket joinRequestPacket)
        {
            _logger.Error("Invalid packet from client.");
            await socket.CloseOutputAsync(WebSocketCloseStatus.InvalidPayloadData, "", CancellationToken.None);
            socketFinishedTcs.SetResult();
            return;
        }

        if (_connectedClients.Any(i => i.UserInfo.UserId == joinRequestPacket.UserId))
        {
            var bytes = new JoinResponsePacket(false, "You are logged in from another location!").SerializeToBytes();
            
            await socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            await socket.CloseOutputAsync(WebSocketCloseStatus.InternalServerError, "", CancellationToken.None);
            socketFinishedTcs.SetResult();
            return;
        }
        
        var userInfo = _userData.UpdateUserDataOnLogin(joinRequestPacket.UserId, joinRequestPacket.UserName);

        var client = new ConnectedClient(socket, userInfo, socketFinishedTcs, _logger);

        if (client.UserInfo.Banned)
        {
            await client.SendPacket(new JoinResponsePacket(false, "You have been banned from CompCube"));
            await client.Disconnect();
            return;
        }

        var queue = _queueManager.GetQueueFromName(joinRequestPacket.Queue);

        if (queue == null)
        {
            await client.SendPacket(new JoinResponsePacket(false, "Invalid queue"));
            await client.Disconnect();
            return;
        }
        
        await client.SendPacket(new JoinResponsePacket(true, ""));
        
        queue.AddClientToPool(client);
        
        _connectedClients.Add(client);
        client.OnDisconnected += OnDisconnected;
        
        _logger.Info($"User {client.UserInfo.Username} ({client.UserInfo.UserId}) joined queue {queue.QueueName}");
    }

    private async Task PollAllClients()
    {
        while (true)
        {
            await Task.Delay(5000);
            
            var clientsToPoll = _connectedClients.ToArray();

            foreach (var client in clientsToPoll)
            {
                try
                {
                    if (client.IsConnectionAlive)
                        continue;
                    await client.Disconnect();
                    _logger.Info($"disconnected {client.UserInfo.Username} via polling");
                }
                catch (Exception e)
                {
                    _logger.Error($"Client {client.UserInfo.UserId} could not be polled for disconnection! {e}");
                }
            }
            
            // _logger.Info($"polled {clientsToPoll.Length} clients");
        }
    }

    

    private void OnDisconnected(IConnectedClient client)
    {
        client.OnDisconnected -= OnDisconnected;
        
        _connectedClients.Remove(client);
        _logger.Info($"{client.UserInfo.Username} ({client.UserInfo.UserId}) disconnected");
    }
}