namespace InfiniteGambler.Constants;

public static class GameFactoryConstants
{
    public const double MinWinningOdds = 1d / 50_000_000;
    public const double MaxWinningOdds = 1d / 10_000;
    public const decimal MinBetCost = 1;
    public const decimal MaxBetCost = 50;
    public const decimal BetCostQuantization = 0.5m;
    public const decimal MinPrize = 1000;
    public const decimal MaxPrize = 1_000_000;
}
