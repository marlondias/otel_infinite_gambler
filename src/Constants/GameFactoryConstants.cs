namespace InfiniteGambler.Constants;

public static class GameFactoryConstants
{
    public const double MinWinningOdds = MaxWinningOdds / 100;
    public const double MaxWinningOdds = 1d / 1_000_000;
    public const decimal MinBetCost = 1;
    public const decimal MaxBetCost = 100;
    public const decimal BetCostQuantization = 0.5m;
    public const decimal MinPrize = 100;
    public const decimal MaxPrize = MinPrize;
}
