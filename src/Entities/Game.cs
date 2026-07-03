namespace InfiniteGambler.Entities;

public class Game
{
    public required string Name { get; init; }
    public required double Odds { get; init; }
    public required decimal BetCost { get; init; }
    public required decimal Prize { get; init; }
    public int BetsCount { get; private set; }
    public int WinnersCount { get; private set; }

    public void IncrementBetsCount() => BetsCount++;

    public void IncrementWinnersCount() => WinnersCount++;

    public override string ToString()
    {
        return string.Join(
            ' ',
            [
                $"Game.Name=\"{Name}\"",
                $"Game.Odds={Odds}",
                $"Game.BetCost={Math.Round(BetCost, 2)}",
                $"Game.Prize={Math.Round(Prize, 2)}",
            ]
        );
    }
}
