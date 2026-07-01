namespace InfiniteGambler.Factories;

using Bogus;
using InfiniteGambler.Constants;
using InfiniteGambler.Entities;
using Microsoft.Extensions.Logging;

public class PlayerFactory(ILogger logger, Faker faker)
{
    private readonly ILogger _logger = logger;
    private readonly Faker _faker = faker;

    public Player Create()
    {
        var player = new Player { Name = _faker.Name.FullName() };
        var minInitialCash = GameFactoryConstants.MaxBetCost * 1000;
        player.AddCashBalance(_faker.Random.Decimal(minInitialCash, minInitialCash * 1000));

        _logger.LogInformation(
            $"A player was created. Name={player.Name} CashBalance={player.CashBalance}."
        );

        return player;
    }
}
