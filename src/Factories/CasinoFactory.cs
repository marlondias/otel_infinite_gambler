namespace InfiniteGambler.Factories;

using System.Collections.Immutable;
using System.Globalization;
using Bogus;
using InfiniteGambler.Constants;
using InfiniteGambler.Entities;
using Microsoft.Extensions.Logging;

public sealed class CasinoFactory(
    ILogger<CasinoFactory> logger,
    Faker faker,
    GameFactory gameFactory
)
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
            Name = GenerateRandomName(),
            Games = _gameFactory.Create(amountOfGames).ToImmutableArray(),
        };

        _logger.LogDebug(
            $"A casino was created. Name={casino.Name} AmountOfGames={casino.Games.Length}."
        );

        return casino;
    }

    public Casino[] Create(int amount)
    {
        return Enumerable.Range(0, amount).Select(i => Create()).ToArray();
    }

    private string GenerateRandomName()
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
            $"{_faker.Commerce.Color()} {_faker.Hacker.Noun()} {_faker.PickRandom(_casinoSuffixes)}"
        );
    }
}
