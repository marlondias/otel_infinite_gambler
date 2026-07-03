namespace InfiniteGambler.Constants;

public static class PlayerFactoryConstants
{
    private const decimal AverageBetCost =
        (GameFactoryConstants.MaxBetCost + GameFactoryConstants.MinBetCost) / 2;
    public const decimal MinInitialCash = AverageBetCost * 1000;
    public const decimal MaxInitialCash = MinInitialCash * 10;
}
