using System.Diagnostics.Metrics;

namespace InfiniteGambler.Instrumentation;

public sealed class SimulationMetrics
{
    private readonly Counter<long> _simulationsStarted;
    private readonly Histogram<double> _simulationDurationMs;
    private readonly Histogram<double> _simulationRounds;
    private readonly Histogram<long> _betsPlaced;
    private readonly Histogram<long> _betsWon;
    private readonly Histogram<double> _cheapestBetCost;
    private readonly Histogram<double> _priceOfCheapestCasino;
    private readonly Histogram<long> _amountOfGames;

    public SimulationMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(Instrumentation.MeterName);

        _simulationsStarted = meter.CreateCounter<long>(
            "simulation.started",
            description: "Number of simulations started"
        );

        _simulationDurationMs = meter.CreateHistogram<double>(
            "simulation.duration",
            unit: "ms",
            description: "Duration of a full simulation run"
        );

        _simulationRounds = meter.CreateHistogram<double>(
            "simulation.rounds",
            description: "Total rounds of a full simulation run"
        );

        _betsPlaced = meter.CreateHistogram<long>(
            "simulation.bets.totalPlaced",
            description: "Number of game bets placed in a simulation."
        );

        _betsWon = meter.CreateHistogram<long>(
            "simulation.bets.totalWon",
            description: "Number of game bets won in a simulation."
        );

        _cheapestBetCost = meter.CreateHistogram<double>(
            "simulation.bets.cheapestCost",
            unit: "$",
            description: "Cost for betting on the cheapest game in a simulation."
        );

        _priceOfCheapestCasino = meter.CreateHistogram<double>(
            "simulation.cheapestCasino",
            unit: "$",
            description: "Price for purchasing the cheapest of all casinos in a simulation."
        );

        _amountOfGames = meter.CreateHistogram<long>(
            "simulation.amountOfGames",
            description: "Number of games available in a simulation."
        );
    }

    public void SimulationCreated(
        int amountOfGames,
        decimal cheapestBetCost,
        decimal priceOfCheapestCasino
    )
    {
        _amountOfGames.Record(amountOfGames);
        _cheapestBetCost.Record((double)cheapestBetCost);
        _priceOfCheapestCasino.Record((double)priceOfCheapestCasino);
    }

    public void SimulationStarted() => _simulationsStarted.Add(1);

    public void SimulationCompleted(double durationMs, long rounds, long betsPlaced, long betsWon)
    {
        _simulationDurationMs.Record(durationMs);
        _simulationRounds.Record(rounds);
        _betsPlaced.Record(betsPlaced);
        _betsWon.Record(betsWon);
    }
}
