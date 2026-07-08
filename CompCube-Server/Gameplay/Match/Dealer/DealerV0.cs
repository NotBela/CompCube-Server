using CompCube_Models.Models.Map;
using CompCube_Server.Interfaces;
using CompCube_Server.SQL;

namespace CompCube_Server.Gameplay.Match.Dealer;

public class DealerV0(MapData mapData) : IDealer
{
    private static readonly Random _random = new();
    
    private List<VotingMap> _discardedOrAlreadyPulled = [];
    
    public VotingMap[] PullNewCards(int count)
    {
        var maps = mapData.GetAllMaps(_discardedOrAlreadyPulled.ToList());

        var shuffled = maps.OrderBy(_ => _random.Next()).ToList();

        var pulledMaps = shuffled.DistinctBy(i => i.Hash).Take(count).ToArray();
        
        AddDiscarded(pulledMaps);

        return pulledMaps;
    }

    public void AddDiscarded(VotingMap[] disallowedMaps)
    {
        _discardedOrAlreadyPulled = _discardedOrAlreadyPulled.Concat(disallowedMaps).ToList();
    }

    public VotingMap[] CompleteDeck(VotingMap[] currentDeck, int targetSize)
    {
        if (currentDeck.Length == targetSize)
            return currentDeck;
        
        var cardsToPull = Math.Abs(currentDeck.Length - targetSize);
        
        var newCards = PullNewCards(cardsToPull);
        
        var cards = currentDeck.Concat(newCards).DistinctBy(i => i.Hash).ToArray();
        
        return CompleteDeck(cards, targetSize);
    }
}