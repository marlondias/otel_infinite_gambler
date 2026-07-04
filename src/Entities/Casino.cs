using System.Collections.Immutable;
using Microsoft.Extensions.Logging;

namespace InfiniteGambler.Entities;

public class Casino(ILogger<Casino> logger)
{
    private readonly ILogger<Casino> _logger = logger;
    public required string Name { get; init; }
    public required ImmutableArray<Game> Games { get; init; }
    public required decimal PurchasePrice { get; init; }
    public decimal MoneyReceivedInBets { get; private set; }
    public decimal MoneyGivenInPrizes { get; private set; }

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
        _logger.LogDebug(
            $"A player placed a bet. PlayerName={player.Name} GameName={Games[gameIndex].Name} BetCost={Games[gameIndex].BetCost}."
        );

        bool isWinner = Random.Shared.NextDouble() < Games[gameIndex].Odds;
        if (isWinner)
        {
            Games[gameIndex].IncrementWinnersCount();
            MoneyGivenInPrizes += Games[gameIndex].Prize;
            player.AddCashBalance(Games[gameIndex].Prize);
            _logger.LogDebug(
                $"A player won a prize. PlayerName={player.Name} GameName={Games[gameIndex].Name} Prize={Games[gameIndex].Prize}."
            );
        }
    }

    public override string ToString()
    {
        return string.Join(
            ' ',
            [
                $"Casino.Name=\"{Name}\"",
                $"Casino.GamesCount={Games.Length}",
                $"Casino.PurchasePrice={Math.Round(PurchasePrice, 2)}",
                $"Casino.MoneyReceivedInBets={Math.Round(MoneyReceivedInBets, 2)}",
                $"Casino.MoneyGivenInPrizes={Math.Round(MoneyGivenInPrizes, 2)}",
            ]
        );
    }
}
