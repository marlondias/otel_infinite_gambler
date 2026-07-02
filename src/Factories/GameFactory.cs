namespace InfiniteGambler.Factories;

using System.Globalization;
using Bogus;
using InfiniteGambler.Constants;
using InfiniteGambler.Entities;
using Microsoft.Extensions.Logging;

public sealed class GameFactory(ILogger<GameFactory> logger, Faker faker)
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
    private readonly ILogger<GameFactory> _logger = logger;
    private readonly Faker _faker = faker;

    public Game Create()
    {
        var odds = GenerateRandomOdds();
        var game = new Game
        {
            Name = GenerateRandomName(),
            BetCost = GenerateRandomBetCost(),
            Odds = odds,
            Payout = GenerateRandomPayout(odds),
        };

        _logger.LogDebug($"A game was created. Name={game.Name} Payout={game.Payout}.");

        return game;
    }

    public Game[] Create(int amount)
    {
        return Enumerable.Range(0, amount).Select(i => Create()).ToArray();
    }

    private string GenerateRandomName()
    {
        var color = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_faker.Commerce.Color());
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

    private double GenerateRandomOdds()
    {
        return _faker.Random.Double(
            GameFactoryConstants.MinWinningOdds,
            GameFactoryConstants.MaxWinningOdds
        );
    }

    private decimal GenerateRandomPayout(double oddsOfWinning)
    {
        var oddsRatio = oddsOfWinning / GameFactoryConstants.MaxWinningOdds;
        var prizeDelta = GameFactoryConstants.MaxPrize - GameFactoryConstants.MinPrize;
        var minPrizeOffset = prizeDelta * (decimal)(1d - oddsRatio);

        return _faker.Random.Decimal(
            GameFactoryConstants.MinPrize + minPrizeOffset,
            GameFactoryConstants.MaxPrize
        );
    }
}
