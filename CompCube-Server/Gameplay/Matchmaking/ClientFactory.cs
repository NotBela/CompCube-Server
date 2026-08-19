using System.Net.WebSockets;
using CompCube_Models.Models.ClientData;
using CompCube_Server.Interfaces;
using CompCube_Server.Networking.Client;

namespace CompCube_Server.Gameplay.Matchmaking;

public class ClientFactory(IServiceProvider services)
{
    public IConnectedClient Create(UserInfo userInfo, WebSocket socket, TaskCompletionSource finishedTask)
    {
        var client = ActivatorUtilities.CreateInstance<ConnectedClient>(services);
        
        client.Init(socket, userInfo, finishedTask);
        return client;
    }
}