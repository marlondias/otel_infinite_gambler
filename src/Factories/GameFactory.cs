namespace InfiniteGambler.Factories;

using Bogus;
using InfiniteGambler.Constants;
using InfiniteGambler.Entities;
using Microsoft.Extensions.Logging;

public class GameFactory(ILogger logger, Faker faker)
{
    private static string[] _gameTypes =
    [
        "Ace",
        "Balls",
        "Bet",
        "Bingo",
        "Cards",
        "Dice",
        "Guess",
        "Holes",
        "Jack",
        "Poker",
        "Roulette",
        "Slots",
        "Ticket",
        "Wheel",
    ];
    private static string[] _gameSuffixes =
    [
        "Chance",
        "Champions",
        "Destiny",
        "Enchantment",
        "Fate",
        "Fortune",
        "Glory",
        "Legends",
        "Luck",
        "Prosperity",
        "Rewards",
        "Riches",
        "Triumph",
        "Wonders",
    ];
    private readonly ILogger _logger = logger;
    private readonly Faker _faker = faker;

    public Game Create()
    {
        var winningOdds = _faker.Random.Double(
            GameFactoryConstants.MinWinningOdds,
            GameFactoryConstants.MaxWinningOdds
        );

        double oddsRatio = winningOdds / GameFactoryConstants.MaxWinningOdds;
        decimal prizeDelta = GameFactoryConstants.MaxPrize - GameFactoryConstants.MinPrize;
        decimal minPrizeLowerBound =
            GameFactoryConstants.MinPrize + (prizeDelta * (decimal)(1d - oddsRatio));
        var prize = _faker.Random.Decimal(minPrizeLowerBound, GameFactoryConstants.MaxPrize);

        var game = new Game
        {
            Name = GenerateRandomName(),
            BetCost = GenerateRandomBetCost(),
            Odds = winningOdds,
            Payout = prize,
        };

        _logger.LogInformation($"A game was created. Name={game.Name}.");

        return game;
    }

    private string GenerateRandomName()
    {
        var color = _faker.Commerce.Color();
        var type = _faker.PickRandom(_gameTypes);
        var suffix = _faker.PickRandom(_gameSuffixes);

        return $"{color} {type} of {suffix}";
    }

    private decimal GenerateRandomBetCost()
    {
        var betCost = _faker.Random.Decimal(
            GameFactoryConstants.MinBetCost,
            GameFactoryConstants.MaxBetCost
        );

        return Math.Floor(betCost / GameFactoryConstants.BetCostQuantization)
            * GameFactoryConstants.BetCostQuantization;
    }
}
