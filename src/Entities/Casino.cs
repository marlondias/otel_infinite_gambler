namespace InfiniteGambler.Entities;

public class Casino
{
    public required string Name { get; init; }
    public required Game[] Games { get; init; }
    public required decimal PurchasePrice { get; init; }
}
