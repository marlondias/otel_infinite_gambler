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
    public decimal MoneyReceivedInBets { get; private set; }
    public decimal MoneyGivenInPrizes { get; private set; }

    public int[] GetIndexesOfGamesWithinBudget(decimal budget)
    {
        return Games.Index().Where(x => x.Item.BetCost <= budget).Select(x => x.Index).ToArray();
    }

    public void PlayGame(int gameIndex, Player player)
    {
        if (gameIndex < 0 || gameIndex > Games.Length)
            throw new ArgumentOutOfRangeException(
                $"Index of game is invalid in this casino. GameIndex={gameIndex}"
            );

        if (player.CashBalance < Games[gameIndex].BetCost)
            throw new InvalidOperationException(
                "Player does not have enough money to play this game."
            );

        Games[gameIndex].IncrementBetsCount();
        player.SubtractFromCashBalance(Games[gameIndex].BetCost);
        MoneyReceivedInBets += Games[gameIndex].BetCost;

        bool isWinner = Random.Shared.NextDouble() < Games[gameIndex].Odds;
        if (isWinner)
        {
            Games[gameIndex].IncrementWinnersCount();
            MoneyGivenInPrizes += Games[gameIndex].Payout;
            player.AddCashBalance(Games[gameIndex].Payout);
        }
    }
}
