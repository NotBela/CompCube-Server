using CompCube_Models.Models.ClientData;
using CompCube_Models.Models.Packets;
using CompCube_Models.Models.Packets.UserPackets;
using CompCube_Server.Interfaces;

namespace CompCube_Server.Networking.Client;

public class DummyConnectedClient(UserInfo userInfo) : IConnectedClient
{
    public event Action<DiscardMapsPacket, IConnectedClient>? OnUserDiscardedMaps;
    public event Action<MapSelectionPacket, IConnectedClient>? OnMapSelection;
    public event Action<ScoreSubmissionPacket, IConnectedClient>? OnScoreSubmission;
    public event Action<IConnectedClient>? OnDisconnected;

    public bool IsConnectionAlive => true;
    public UserInfo UserInfo => userInfo;

    public Task SendPacket(ServerPacket packet)
    {
        switch (packet.PacketType)
        {
            case ServerPacket.ServerPacketTypes.PlayerSelectedMap:
                OnScoreSubmission?.Invoke(new ScoreSubmissionPacket(10000, 10000, true, 0, true), this);
                break;
        }

        return Task.CompletedTask;
    }

    public void Disconnect()
    {
        OnDisconnected?.Invoke(this);
    }
}