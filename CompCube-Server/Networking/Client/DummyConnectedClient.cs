using CompCube_Models.Models.ClientData;
using CompCube_Models.Models.Packets;
using CompCube_Models.Models.Packets.ServerPackets;
using CompCube_Models.Models.Packets.UserPackets;
using CompCube_Server.Data;
using CompCube_Server.Interfaces;

namespace CompCube_Server.Networking.Client;

public class DummyConnectedClient(UserInfo userInfo, MapData mapData) : IConnectedClient
{
    public event Action<DiscardMapsPacket, IConnectedClient>? OnUserDiscardedMaps;
    public event Action<MapSelectionPacket, IConnectedClient>? OnMapSelection;
    public event Action<ScoreSubmissionPacket, IConnectedClient>? OnScoreSubmission;
    public event Action<IConnectedClient>? OnDisconnected;

    public bool IsConnectionAlive => true;
    public UserInfo UserInfo => userInfo;

    public async Task SendPacket(ServerPacket packet)
    {
        await Task.Delay(70);

        SendPacketInternal(packet);
    }

    private async Task SendPacketInternal(ServerPacket packet)
    {
        if (packet.PacketType == ServerPacket.ServerPacketTypes.MatchFinished)
        {
            Disconnect();
            return;
        }
        
        await Task.Delay(5000);
        
        switch (packet.PacketType)
        {
            case ServerPacket.ServerPacketTypes.MatchCreated:
                OnUserDiscardedMaps?.Invoke(new DiscardMapsPacket([]), this);
                break;
            case ServerPacket.ServerPacketTypes.PlayerSelectedMap:
                OnScoreSubmission?.Invoke(new ScoreSubmissionPacket(200000, 1000000, true, 0, false), this);
                await Task.Delay(3500);
                Disconnect();
                break;
            case ServerPacket.ServerPacketTypes.StartPickPhase:
                var pickPhasePacket = packet as StartPickPhasePacket;
                if (pickPhasePacket!.IsOwnPick)
                {
                    await Task.Delay(15000);
                    OnMapSelection?.Invoke(new MapSelectionPacket(mapData.GetAllMaps().First()), this);
                    await Task.Delay(5000);
                    OnScoreSubmission?.Invoke(new ScoreSubmissionPacket(200000, 1000000, true, 0, false), this);
                }
                break;
        }
    }

    public Task Disconnect()
    {
        OnDisconnected?.Invoke(this);
        return Task.CompletedTask;
    }

    public Task DisconnectAbruptlyAsync(string reason)
    {
        Disconnect();
        return Task.CompletedTask;
    }
}