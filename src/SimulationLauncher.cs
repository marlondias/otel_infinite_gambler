using System.Diagnostics;
using InfiniteGambler.Factories;
using InfiniteGambler.Instrumentation;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InfiniteGambler;

public class SimulationLauncher(
    IHostApplicationLifetime lifetime,
    ILogger<SimulationLauncher> logger,
    SimulationMetrics metrics,
    PlayerFactory playerFactory,
    CasinoFactory casinoFactory
) : BackgroundService
{
    private readonly IHostApplicationLifetime _lifetime = lifetime;
    private readonly ILogger<SimulationLauncher> _logger = logger;
    private readonly SimulationMetrics _metrics = metrics;
    private readonly PlayerFactory _playerFactory = playerFactory;
    private readonly CasinoFactory _casinoFactory = casinoFactory;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var player = _playerFactory.Create();
            var casinos = _casinoFactory.Create(10);

            _logger.LogInformation("Simulation started.");
            _metrics.SimulationStarted();
            var sw = Stopwatch.StartNew();

            _metrics.SimulationCompleted(sw.Elapsed.TotalMilliseconds);
            _logger.LogInformation("Simulation ended.");
            return Task.CompletedTask;
        }
        finally
        {
            _lifetime.StopApplication();
        }
    }
}
