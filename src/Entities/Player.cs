namespace InfiniteGambler.Entities;

public class Player
{
    public required string Name { get; init; }
    public decimal CashBalance { get; private set; }
}
