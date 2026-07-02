using System.Collections.Immutable;

namespace InfiniteGambler.Entities;

public class Casino
{
    public required string Name { get; init; }
    public required ImmutableArray<Game> Games { get; init; }
    public decimal PurchasePrice
    {
        get => Games.Max(g => g.Payout) * 1.5m;
    }

    public int[] GetIndexesOfGamesWithinBudget(decimal budget)
    {
        return Games.Index().Where(x => x.Item.BetCost <= budget).Select(x => x.Index).ToArray();
    }
}
