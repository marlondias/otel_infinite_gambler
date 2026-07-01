namespace InfiniteGambler.Factories;

using System.Collections.Immutable;
using Bogus;
using InfiniteGambler.Constants;
using InfiniteGambler.Entities;
using Microsoft.Extensions.Logging;

public class CasinoFactory(ILogger<CasinoFactory> logger, Faker faker, GameFactory gameFactory)
{
    private static string[] _casinoSuffixes = ["Club", "Hall", "Lounge", "Palace"];
    private readonly ILogger<CasinoFactory> _logger = logger;
    private readonly Faker _faker = faker;
    private readonly GameFactory _gameFactory = gameFactory;

    public Casino Create()
    {
        var amountOfGames = _faker.Random.Int(
            CasinoFactoryConstants.MinAmountOfGames,
            CasinoFactoryConstants.MaxAmountOfGames
        );

        var casino = new Casino
        {
            Name =
                $"{_faker.Commerce.Color()} {_faker.Hacker.Noun()} {_faker.PickRandom(_casinoSuffixes)}",
            Games = Enumerable
                .Range(0, amountOfGames)
                .Select(i => _gameFactory.Create())
                .ToImmutableArray(),
        };

        _logger.LogInformation(
            $"A casino was created. Name={casino.Name} AmountOfGames={casino.Games.Length}."
        );

        return casino;
    }
}
