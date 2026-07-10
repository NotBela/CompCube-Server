using CompCube_Models.Models.ClientData;
using CompCube_Models.Models.Packets;
using CompCube_Models.Models.Packets.ServerPackets;
using CompCube_Models.Models.Packets.UserPackets;
using CompCube_Server.Interfaces;
using CompCube_Server.SQL;

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
                OnScoreSubmission?.Invoke(new ScoreSubmissionPacket(400000, 400000, true, 0, false), this);
                break;
            case ServerPacket.ServerPacketTypes.StartPickPhase:
                var pickPhasePacket = packet as StartPickPhasePacket;
                if (pickPhasePacket!.IsOwnPick)
                {
                    await Task.Delay(15000);
                    OnMapSelection?.Invoke(new MapSelectionPacket(mapData.GetAllMaps().First(i => i.Hash == "6abbfe0b5659600b33cafbfe994cf40c8f97e806")), this);
                    await Task.Delay(5000);
                    OnScoreSubmission?.Invoke(new ScoreSubmissionPacket(400000, 400000, true, 0, false), this);
                }
                break;
        }
    }

    public void Disconnect()
    {
        OnDisconnected?.Invoke(this);
    }
}