namespace InfiniteGambler.Simulation;

public record AffordableGameSummary(
    int CasinoIndex,
    int GameIndex,
    double Odds,
    decimal ReturnOnInvestment
);
