using System.Net.WebSockets;
using System.Text;
using CompCube_Models.Models.Packets;
using CompCube_Models.Models.Packets.ServerPackets;
using CompCube_Models.Models.Packets.UserPackets;
using CompCube_Server.Config;
using CompCube_Server.Data;
using CompCube_Server.Interfaces;
using CompCube_Server.Networking.Client;
using Microsoft.AspNetCore.Mvc;

namespace CompCube_Server.Gameplay.Matchmaking;

public class ConnectionManager : ControllerBase
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

    [Route("queue/{queueName}")]
    public async Task HandleWebSocket(string queueName)
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        var websocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

        if (!HttpContext.Request.Headers.TryGetValue("UserId", out var userIdHeader) || !HttpContext.Request.Headers.TryGetValue("UserName", out var usernameHeader))
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }
        
        var userId = userIdHeader.First() ?? throw new Exception("No UserId!");
        var username = usernameHeader.First() ?? throw new Exception("No UserName!");
        
        var tcs = new TaskCompletionSource();

        var userInfo = _userData.UpdateUserDataOnLogin(userId, username);

        var connectedClient = _clientFactory.Create(userInfo, websocket, tcs);

        if (_connectedClients.Any(i => i.UserInfo.UserId == userId))
        {
            await connectedClient.DisconnectAbruptlyAsync("You are logged in from another location!");
            return;
        }
        
        if (userInfo.Banned)
        {
            await connectedClient.DisconnectAbruptlyAsync("You have been banned from CompCube.");
            return;
        }

        if (_timeoutManager.IsUserTimedOut(userId))
        {
            await connectedClient.DisconnectAbruptlyAsync($"You have been timed out temporarily.\nTry again in {(int) _timeoutManager.GetRemainingTimeoutTime(userId).TotalMinutes + 1} minute(s)");
            return;
        }

        if (_config.WhitelistEnabled && !_config.WhitelistedIds.Contains(userId))
        {
            await connectedClient.DisconnectAbruptlyAsync("You are not whitelisted!");
            return;
        }
        
        var queue = _queueManager.GetQueueFromName(queueName);

        if (queue == null)
        {
            await connectedClient.DisconnectAbruptlyAsync("Invalid Queue!");
            return;
        }
        
        queue.AddClientToPool(connectedClient);
        
        await tcs.Task;
    }

    private void OnDisconnected(IConnectedClient client)
    {
        client.OnDisconnected -= OnDisconnected;
        
        _connectedClients.Remove(client);
        _logger.LogInformation("{UserInfoUsername} ({UserInfoUserId}) disconnected", client.UserInfo.Username, client.UserInfo.UserId);
    }
}