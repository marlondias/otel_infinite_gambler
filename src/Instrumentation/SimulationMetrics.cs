using System.Diagnostics.Metrics;

namespace InfiniteGambler.Instrumentation;

public sealed class SimulationMetrics
{
    private readonly Counter<long> _simulationsStarted;
    private readonly Counter<long> _spinsPlayed;
    private readonly Histogram<double> _simulationDurationMs;

    public SimulationMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(Instrumentation.MeterName);

        _simulationsStarted = meter.CreateCounter<long>(
            "simulation.started",
            description: "Number of simulations started"
        );

        _spinsPlayed = meter.CreateCounter<long>(
            "simulation.spins.played",
            description: "Number of spins played across all simulations"
        );

        _simulationDurationMs = meter.CreateHistogram<double>(
            "simulation.duration",
            unit: "ms",
            description: "Duration of a full simulation run"
        );
    }

    public void SimulationStarted() => _simulationsStarted.Add(1);

    public void SpinsPlayed(long count) => _spinsPlayed.Add(count);

    public void SimulationCompleted(double durationMs) => _simulationDurationMs.Record(durationMs);
}
