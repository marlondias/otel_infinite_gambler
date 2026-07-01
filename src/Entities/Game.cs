namespace InfiniteGambler.Entities;

public class Game
{
    public required string Name { get; init; }
    public required double Odds { get; init; }
    public required decimal BetCost { get; init; }
    public required decimal Payout { get; init; }
}
