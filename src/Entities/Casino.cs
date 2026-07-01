namespace InfiniteGambler.Entities;

public class Casino
{
    public required string Name { get; init; }
    public required Game[] Games { get; init; }
    public decimal PurchasePrice
    {
        get => Games.Max(g => g.Payout) * 1.5m;
    }
}
