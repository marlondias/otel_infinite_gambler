namespace InfiniteGambler.Simulation;

public record GameSummary(
    int CasinoIndex,
    int GameIndex,
    decimal BetCost,
    decimal Prize,
    double Odds
);
