namespace InfiniteGambler.Entities;

public class Player
{
    public required string Name { get; init; }
    public decimal CashBalance { get; private set; }

    public void AddCashBalance(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive.");

        CashBalance += amount;
    }

    public void SubtractFromCashBalance(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive.");

        CashBalance -= amount;
    }

    public override string ToString()
    {
        return $"Player.Name=\"{Name}\" Player.CashBalance={Math.Round(CashBalance, 2)}";
    }
}
