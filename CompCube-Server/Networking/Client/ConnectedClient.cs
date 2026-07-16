using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using CompCube_Models.Models.ClientData;
using CompCube_Models.Models.Packets;
using CompCube_Models.Models.Packets.ServerPackets;
using CompCube_Models.Models.Packets.UserPackets;
using CompCube_Server.Interfaces;
using CompCube_Server.Logging;

namespace CompCube_Server.Networking.Client;

public class ConnectedClient : IConnectedClient, IAsyncDisposable
{
    private readonly Logger _logger;
    
    private readonly WebSocket _client;
    private readonly TaskCompletionSource _socketFinishedTcs;

    public event Action<DiscardMapsPacket, IConnectedClient>? OnUserDiscardedMaps;
    public event Action<MapSelectionPacket, IConnectedClient>? OnMapSelection;
    public event Action<ScoreSubmissionPacket, IConnectedClient>? OnScoreSubmission;
    public event Action<IConnectedClient>? OnDisconnected;

    public UserInfo UserInfo { get; }
    
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public ConnectedClient(WebSocket client, UserInfo userInfo, TaskCompletionSource socketFinishedTcs, Logger logger)
    {
        _client = client;
        UserInfo = userInfo;
        _logger = logger;
        _socketFinishedTcs = socketFinishedTcs;

        Task.Factory.StartNew(ListenToClient, TaskCreationOptions.LongRunning);
    }

    private async Task ListenToClient()
    {
        try
        {
            while (true)
            {
                _logger.Info(_client.State.ToString());
                
                var buffer = new byte[4096];
                
                var result = await _client.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationTokenSource.Token);
                Array.Resize(ref buffer, result.Count);
                
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", _cancellationTokenSource.Token);
                    await Disconnect();
                    return;
                }

                var json = Encoding.UTF8.GetString(buffer);

                if (json == "")
                    continue;

                var packetWasDeserialized = UserPacket.TryDeserialize(json, out var packet);

                if (!packetWasDeserialized)
                {
                    _logger.Error($"Failed to deserialize packet from client {UserInfo.UserId}");
                    await Disconnect();
                    return;
                }

                await ProcessRecievedPacket(packet!);
            }
        }
        catch (OperationCanceledException)
        {

        }
        catch (Exception e)
        {
            _logger.Error(e);
        }
    }

    public async Task Disconnect()
    {
        OnDisconnected?.Invoke(this);
        await _client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", _cancellationTokenSource.Token);
        _socketFinishedTcs.SetResult();
    }

    public async Task DisconnectAbruptlyAsync(string reason)
    {
        await SendPacket(new AbruptDisconnectionPacket(reason));
        await Disconnect();
    }

    public bool IsConnectionAlive => _client.State is WebSocketState.Open or WebSocketState.Connecting;

    private async Task ProcessRecievedPacket(UserPacket packet)
    {
        switch (packet.PacketType)
        {
            case UserPacket.UserPacketTypes.DiscardMaps:
                OnUserDiscardedMaps?.Invoke(packet as DiscardMapsPacket ?? throw new InvalidOperationException(), this);
                break;
            case UserPacket.UserPacketTypes.MapSelection:
                OnMapSelection?.Invoke(packet as MapSelectionPacket ?? throw new InvalidOperationException(), this);
                break;
            case UserPacket.UserPacketTypes.ScoreSubmission:
                OnScoreSubmission?.Invoke(packet as ScoreSubmissionPacket ?? throw new InvalidOperationException(), this);
                break;
            default:
                await Disconnect();
                throw new Exception("Unknown packet type!");
        }
    }

    public async Task SendPacket(ServerPacket packet)
    {
        await _client.SendAsync(new ArraySegment<byte>(packet.SerializeToBytes()), WebSocketMessageType.Text, true, _cancellationTokenSource.Token);
    }

    public async ValueTask DisposeAsync()
    {
        await Disconnect();
    }
}