using System.Diagnostics.Metrics;

namespace InfiniteGambler.Instrumentation;

public sealed class SimulationMetrics
{
    private readonly Counter<long> _simulationsStarted;
    private readonly Gauge<double> _simulationDurationMs;
    private readonly Gauge<double> _simulationRounds;
    private readonly Gauge<long> _betsPlaced;
    private readonly Gauge<long> _betsWon;
    private readonly Gauge<double> _cheapestBetCost;
    private readonly Gauge<double> _priceOfCheapestCasino;
    private readonly Gauge<long> _amountOfGames;

    public SimulationMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(Instrumentation.MeterName);

        _simulationsStarted = meter.CreateCounter<long>(
            "simulation.started",
            description: "Number of simulations started"
        );

        _simulationDurationMs = meter.CreateGauge<double>(
            "simulation.duration",
            unit: "ms",
            description: "Duration of a full simulation run"
        );

        _simulationRounds = meter.CreateGauge<double>(
            "simulation.rounds",
            description: "Total rounds of a full simulation run"
        );

        _betsPlaced = meter.CreateGauge<long>(
            "simulation.bets.totalPlaced",
            description: "Number of game bets placed in a simulation."
        );

        _betsWon = meter.CreateGauge<long>(
            "simulation.bets.totalWon",
            description: "Number of game bets won in a simulation."
        );

        _cheapestBetCost = meter.CreateGauge<double>(
            "simulation.bets.cheapestCost",
            unit: "$",
            description: "Cost for betting on the cheapest game in a simulation."
        );

        _priceOfCheapestCasino = meter.CreateGauge<double>(
            "simulation.cheapestCasino",
            unit: "$",
            description: "Price for purchasing the cheapest of all casinos in a simulation."
        );

        _amountOfGames = meter.CreateGauge<long>(
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
