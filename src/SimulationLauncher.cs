using InfiniteGambler.Factories;
using Microsoft.Extensions.Logging;

namespace InfiniteGambler;

public sealed class SimulationLauncher(
    ILogger<SimulationLauncher> logger,
    PlayerFactory playerFactory,
    CasinoFactory casinoFactory
)
{
    private readonly ILogger<SimulationLauncher> _logger = logger;
    private readonly PlayerFactory _playerFactory = playerFactory;
    private readonly CasinoFactory _casinoFactory = casinoFactory;

    public void Run()
    {
        _logger.LogInformation("Simulation started.");

        var player = _playerFactory.Create();
        var casinos = _casinoFactory.Create(10);

        _logger.LogInformation("Simulation ended.");
    }
}
