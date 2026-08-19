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

[ApiExplorerSettings(IgnoreApi = true)]
public partial class ConnectionManager(
    UserData userData,
    ILogger<ConnectionManager> logger,
    QueueManager queueManager,
    ClientFactory clientFactory,
    TimeoutManager timeoutManager,
    ConfigHelper config)
    : ControllerBase
{
    private readonly List<IConnectedClient> _connectedClients = [];

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

        var userInfo = userData.UpdateUserDataOnLogin(userId, username);

        var connectedClient = clientFactory.Create(userInfo, websocket, tcs);

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

        if (timeoutManager.IsUserTimedOut(userId))
        {
            await connectedClient.DisconnectAbruptlyAsync($"You have been timed out temporarily.\nTry again in {(int) timeoutManager.GetRemainingTimeoutTime(userId).TotalMinutes + 1} minute(s)");
            return;
        }

        if (config.WhitelistEnabled && !config.WhitelistedIds.Contains(userId))
        {
            await connectedClient.DisconnectAbruptlyAsync("You are not whitelisted!");
            return;
        }
        
        var queue = queueManager.GetQueueFromName(queueName);

        if (queue == null)
        {
            await connectedClient.DisconnectAbruptlyAsync("Invalid Queue!");
            return;
        }
        
        queue.AddClientToPool(connectedClient);
        
        _connectedClients.Add(connectedClient);
        connectedClient.OnDisconnected += OnDisconnected;
        
        LogUsernameUseridJoinedQueueQueue(logger, username, userId, queue);
        
        await tcs.Task;
    }

    private void OnDisconnected(IConnectedClient client)
    {
        client.OnDisconnected -= OnDisconnected;
        
        _connectedClients.Remove(client);
        LogUserinfousernameUserinfouseridDisconnected(logger, client.UserInfo.Username, client.UserInfo.UserId);
    }

    [LoggerMessage(LogLevel.Information, "{userName} ({userId}) joined queue {queue}")]
    static partial void LogUsernameUseridJoinedQueueQueue(ILogger<ConnectionManager> logger, string userName, string userId, IQueue queue);

    [LoggerMessage(LogLevel.Information, "{UserInfoUsername} ({UserInfoUserId}) disconnected")]
    static partial void LogUserinfousernameUserinfouseridDisconnected(ILogger<ConnectionManager> logger, string UserInfoUsername, string UserInfoUserId);
}