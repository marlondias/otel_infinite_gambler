namespace InfiniteGambler.Factories;

using Bogus;
using InfiniteGambler.Constants;
using InfiniteGambler.Entities;
using Microsoft.Extensions.Logging;

public sealed class PlayerFactory(ILogger<PlayerFactory> logger, Faker faker)
{
    private readonly ILogger<PlayerFactory> _logger = logger;
    private readonly Faker _faker = faker;

    public Player Create()
    {
        var player = new Player { Name = _faker.Name.FullName() };
        var minInitialCash = GameFactoryConstants.MaxBetCost * 1000;
        var initialCash = Math.Round(_faker.Random.Decimal(minInitialCash, minInitialCash * 1000));
        player.AddCashBalance(initialCash);

        _logger.LogDebug(
            $"A player was created. Name={player.Name} CashBalance={player.CashBalance}."
        );

        return player;
    }
}
