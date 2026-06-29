
namespace InfiniteGambler.Lottery;

public class Game
{
    public string Name { get; init; }
    public GameDifficulty Difficulty { get;}
    public decimal TicketPrice {
        get => Difficulty switch
        {
            GameDifficulty.EASY => 2.5m,
            GameDifficulty.NORMAL => 5m,
            GameDifficulty.HARD => 10m,
            GameDifficulty.EXTREME => 100m,
            _ => throw new NotImplementedException("GameDifficulty value is not supported.")
        };
    }
    public decimal FullPrize {
        get => Difficulty switch
        {
            GameDifficulty.EASY => 10_000,
            GameDifficulty.NORMAL => 100_000,
            GameDifficulty.HARD => 1_000_000,
            GameDifficulty.EXTREME => 100_000_000,
            _ => throw new NotImplementedException("GameDifficulty value is not supported.")
        };
    }
    public ushort[] ValidNumbers { get; init; }
    public ushort AmountOfNumbersToDraw { get; init; }

    public Game(string name, GameDifficulty difficulty)
    {
        Name = name;
        Difficulty = difficulty;

        ushort totalValidNumbers = difficulty switch
        {
            GameDifficulty.EASY => 50,
            GameDifficulty.NORMAL => 100,
            GameDifficulty.HARD => 250,
            GameDifficulty.EXTREME => 1000,
            _ => throw new NotImplementedException("GameDifficulty value is not supported.")
        };
        ValidNumbers = Enumerable.Range(1, totalValidNumbers).Select(n => (ushort) n).ToArray();

        var oddsOfWinning = difficulty switch
        {
            GameDifficulty.EASY => 0.4,
            GameDifficulty.NORMAL => 0.1,
            GameDifficulty.HARD => 0.01,
            GameDifficulty.EXTREME => 0.001,
            _ => throw new NotImplementedException("GameDifficulty value is not supported.")
        };
        AmountOfNumbersToDraw = (ushort) Math.Max(ValidNumbers.Length * oddsOfWinning, 1);
    }

    public ushort[] GenerateDrawNumbers()
    {
        if (ValidNumbers == null || ValidNumbers.Length == 0 || AmountOfNumbersToDraw == 0)
            return Array.Empty<ushort>();

        var take = Math.Min(AmountOfNumbersToDraw, (ushort)ValidNumbers.Length);

        // Fisher-Yates shuffle on a copy of the valid numbers, then take the first `take` items
        var copy = (ushort[])ValidNumbers.Clone();
        var rnd = new Random();
        for (int i = copy.Length - 1; i > 0; i--)
        {
            int j = rnd.Next(i + 1);
            var tmp = copy[i];
            copy[i] = copy[j];
            copy[j] = tmp;
        }

        var result = new ushort[take];
        Array.Copy(copy, result, take);
        return result;
    }

}