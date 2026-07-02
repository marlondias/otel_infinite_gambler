using System.Diagnostics;
using InfiniteGambler.Factories;
using InfiniteGambler.Instrumentation;
using Microsoft.Extensions.Logging;

namespace InfiniteGambler;

public sealed class SimulationLauncher(
    ILogger<SimulationLauncher> logger,
    SimulationMetrics metrics,
    PlayerFactory playerFactory,
    CasinoFactory casinoFactory
)
{
    private readonly ILogger<SimulationLauncher> _logger = logger;
    private readonly SimulationMetrics _metrics = metrics;
    private readonly PlayerFactory _playerFactory = playerFactory;
    private readonly CasinoFactory _casinoFactory = casinoFactory;

    public void Run()
    {
        _metrics.SimulationStarted();
        _logger.LogInformation("Simulation started.");
        var sw = Stopwatch.StartNew();

        var player = _playerFactory.Create();
        var casinos = _casinoFactory.Create(10);

        _metrics.SimulationCompleted(sw.Elapsed.TotalMilliseconds);
        _logger.LogInformation("Simulation ended.");
    }
}
