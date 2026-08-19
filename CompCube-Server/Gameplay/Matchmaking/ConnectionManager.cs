using System.Net.WebSockets;
using System.Text;
using CompCube_Models.Models.Packets;
using CompCube_Models.Models.Packets.ServerPackets;
using CompCube_Models.Models.Packets.UserPackets;
using CompCube_Server.Config;
using CompCube_Server.Data;
using CompCube_Server.Interfaces;
using CompCube_Server.Networking.Client;

namespace CompCube_Server.Gameplay.Matchmaking;

public class ConnectionManager
{
    private readonly UserData _userData;
    private readonly QueueManager _queueManager;
    private readonly ILogger<ConnectionManager> _logger;
    private readonly ClientFactory _clientFactory;
    private readonly TimeoutManager _timeoutManager;
    private readonly ConfigHelper _config;
    
    private readonly List<IConnectedClient> _connectedClients = [];
    
    public ConnectionManager(UserData userData, ILogger<ConnectionManager> logger, QueueManager queueManager, ClientFactory clientFactory, TimeoutManager timeoutManager, ConfigHelper config)
    {
        _userData = userData;
        _logger = logger;
        _queueManager = queueManager;
        _clientFactory = clientFactory;
        _timeoutManager = timeoutManager;
        _config = config;

        // Task.Factory.StartNew(PollAllClients, TaskCreationOptions.LongRunning);
        _logger.LogInformation("Started listening for clients");
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
            _logger.LogError("Could not parse packet from client.");
            await socket.CloseOutputAsync(WebSocketCloseStatus.InvalidPayloadData, "", CancellationToken.None);
            socketFinishedTcs.SetResult();
            return;
        }

        if (packet is not JoinRequestPacket joinRequestPacket)
        {
            _logger.LogError("Invalid packet from client.");
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

        if (_config.WhitelistEnabled && !_config.WhitelistedIds.Contains(joinRequestPacket.UserId))
        {
            var bytes = new  JoinResponsePacket(false, "You are not whitelisted!").SerializeToBytes();
            
            await socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            await socket.CloseOutputAsync(WebSocketCloseStatus.InternalServerError, "", CancellationToken.None);
            socketFinishedTcs.SetResult();
            return;
        }
        
        var userInfo = _userData.UpdateUserDataOnLogin(joinRequestPacket.UserId, joinRequestPacket.UserName);

        var client = _clientFactory.Create(userInfo, socket, socketFinishedTcs);
        
        if (client.UserInfo.Banned)
        {
            await client.SendPacket(new JoinResponsePacket(false, "You have been banned from CompCube"));
            await client.Disconnect();
            return;
        }

        if (_timeoutManager.IsUserTimedOut(joinRequestPacket.UserId))
        {
            await client.SendPacket(new JoinResponsePacket(false, $"You have been timed out temporarily.\nTry again in {((int) _timeoutManager.GetRemainingTimeoutTime(joinRequestPacket.UserId).TotalMinutes) + 1} minute(s)"));
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
        
        _logger.LogInformation("User {UserInfoUsername} ({UserInfoUserId}) joined queue {QueueName}", client.UserInfo.Username, client.UserInfo.UserId, queue.QueueName);
    }

    

    private void OnDisconnected(IConnectedClient client)
    {
        client.OnDisconnected -= OnDisconnected;
        
        _connectedClients.Remove(client);
        _logger.LogInformation("{UserInfoUsername} ({UserInfoUserId}) disconnected", client.UserInfo.Username, client.UserInfo.UserId);
    }
}