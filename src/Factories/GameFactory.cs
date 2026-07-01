namespace InfiniteGambler.Factories;

using Bogus;
using InfiniteGambler.Constants;
using InfiniteGambler.Entities;

public class GameFactory(Faker faker)
{
    private static string[] _gameTypes = new[]
    {
        "Dice",
        "Cards",
        "Wheel",
        "Balls",
        "Guess",
        "Roulette",
        "Slots",
        "Bingo",
        "Bet",
        "Poker",
        "Jack",
        "Ticket",
    };
    private static string[] _gameSuffixes = new[]
    {
        "Chance",
        "Destiny",
        "Enchantment",
        "Fate",
        "Fortune",
        "Glory",
        "Luck",
        "Legends",
        "Prosperity",
        "Rewards",
        "Riches",
        "Triumph",
        "Wonders",
    };
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

        return new Game
        {
            Name =
                $"{_faker.Commerce.Color()} {_faker.PickRandom(_gameTypes)} of {_faker.PickRandom(_gameSuffixes)}",
            BetCost = _faker.Random.Decimal(
                GameFactoryConstants.MinBetCost,
                GameFactoryConstants.MaxBetCost
            ),
            Odds = winningOdds,
            Payout = prize,
        };
    }
}
