using CompCube_Models.Models.Map;

namespace CompCube_Server.Interfaces;

public interface IDealer
{
    public VotingMap[] PullNewCards(int count);

    public void AddDiscarded(VotingMap[] disallowedMaps);
}