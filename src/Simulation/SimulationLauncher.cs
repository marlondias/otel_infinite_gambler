using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using InfiniteGambler.Config;
using InfiniteGambler.Entities;
using InfiniteGambler.Factories;
using InfiniteGambler.Instrumentation;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace InfiniteGambler.Simulation;

public class SimulationLauncher : BackgroundService
{
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ILogger<SimulationLauncher> _logger;
    private readonly SimulationMetrics _metrics;
    private readonly ImmutableArray<Casino> _casinos;
    private readonly ImmutableArray<Player> _players;
    private readonly ImmutableArray<GameSummary> _gameSummaries;
    private readonly int _amountOfGames;
    private readonly int _indexOfCheapestCasino;
    private readonly decimal _bettingCostOfCheapestGame;
    private long _gamblingRounds;

    public SimulationLauncher(
        IHostApplicationLifetime lifetime,
        ILogger<SimulationLauncher> logger,
        SimulationMetrics metrics,
        IOptions<ApplicationConfig> appConfig,
        PlayerFactory playerFactory,
        CasinoFactory casinoFactory
    )
    {
        _lifetime = lifetime;
        _logger = logger;
        _metrics = metrics;
        _casinos = casinoFactory.Create(appConfig.Value.AmountOfCasinos).ToImmutableArray();
        _players = playerFactory.Create(appConfig.Value.AmountOfPlayers).ToImmutableArray();
        _amountOfGames = _casinos.Sum(c => c.Games.Length);

        _indexOfCheapestCasino = _casinos
            .Select((casino, index) => (casino, index))
            .OrderBy(x => x.casino.PurchasePrice)
            .First()
            .index;

        _gameSummaries = _casinos
            .SelectMany(
                (casino, casinoIndex) =>
                    casino.Games.Select(
                        (game, gameIndex) =>
                            new GameSummary(
                                casinoIndex,
                                gameIndex,
                                game.BetCost,
                                game.Prize,
                                game.Odds
                            )
                    )
            )
            .ToImmutableArray();

        _bettingCostOfCheapestGame = _gameSummaries.Min(s => s.BetCost);

        _logger.LogInformation(
            $"Simulation created with {_players.Length} players, {_casinos.Length} casinos and {_amountOfGames} games."
        );
        _logger.LogDebug(
            $"Cheapest casino is worth ${_casinos[_indexOfCheapestCasino].PurchasePrice}."
        );
        _logger.LogDebug($"Cheapest game costs ${_bettingCostOfCheapestGame} per bet.");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _metrics.SimulationCreated(
                _amountOfGames,
                _bettingCostOfCheapestGame,
                _casinos[_indexOfCheapestCasino].PurchasePrice
            );

            _metrics.SimulationStarted();
            _logger.LogInformation("Simulation started.");
            ShowPlayersRanking();

            var sw = Stopwatch.StartNew();
            while (!CanSomePlayerBuyACasino() && CanSomePlayerBet())
            {
                RunGamblingRound();
            }
            sw.Stop();

            if (CanSomePlayerBuyACasino())
                ShowMessageForCasinoPurchased();

            if (!CanSomePlayerBet())
                ShowMessageForAllPlayersBankrupt();

            ShowPlayersRanking();

            var totalBetsPlaced = _casinos.Sum(c => c.Games.Sum(g => g.BetsCount));
            var totalBetsWon = _casinos.Sum(c => c.Games.Sum(g => g.WinnersCount));
            _metrics.SimulationCompleted(
                sw.Elapsed.TotalMilliseconds,
                _gamblingRounds,
                totalBetsPlaced,
                totalBetsWon
            );
            _logger.LogInformation($"Simulation completed after {_gamblingRounds} rounds.");

            return Task.CompletedTask;
        }
        finally
        {
            _lifetime.StopApplication();
        }
    }

    private bool CanSomePlayerBuyACasino()
    {
        var casino = _casinos[_indexOfCheapestCasino];
        return _players.Any(p => p.CashBalance >= casino.PurchasePrice);
    }

    private bool CanSomePlayerBet()
    {
        return _players.Any(p => p.CashBalance >= _bettingCostOfCheapestGame);
    }

    private void ShowMessageForCasinoPurchased()
    {
        var casino = _casinos[_indexOfCheapestCasino];
        var players = _players
            .Where(p => p.CashBalance >= casino.PurchasePrice)
            .OrderByDescending(p => p.CashBalance)
            .ToArray();

        StringBuilder sb = new();
        sb.Append("The improbable just happened before our eyes!");
        sb.Append(players.Length > 1 ? $" {players.Length} lucky players" : " One lucky player");
        sb.Append(" got rich enough to BUY A CASINO!");
        if (players.Length > 1)
        {
            sb.AppendLine();
            sb.Append("(The casino will be sold only to the richest player.)");
        }
        _logger.LogInformation(sb.ToString());
        sb.Clear();

        var winner = players[0];
        _logger.LogInformation($"{winner.Name} is the new owner of the {casino.Name} casino!");
    }

    private void ShowMessageForAllPlayersBankrupt()
    {
        _logger.LogInformation(
            "The bank accounts of all player were drained. Nobody can affort another round."
        );
    }

    private void ShowPlayersRanking()
    {
        StringBuilder sb = new();
        sb.Append("Players Ranking:");

        int order = 1;
        foreach (var p in _players.OrderByDescending(p => p.CashBalance))
        {
            sb.AppendLine();
            sb.Append($"{order} => {p.Name} ($ {Math.Round(p.CashBalance, 2)})");
            order++;
        }

        _logger.LogInformation(sb.ToString());
    }

    private void RunGamblingRound()
    {
        foreach (var player in _players.Where(p => p.CashBalance >= _bettingCostOfCheapestGame))
        {
            var summaries = _gameSummaries
                .Where(s => s.BetCost <= player.CashBalance)
                .OrderByDescending(s => s.Odds);

            foreach (var s in summaries)
            {
                if (s.BetCost > player.CashBalance)
                    continue;

                _casinos[s.CasinoIndex].PlayGame(s.GameIndex, player);
            }
        }
        _gamblingRounds++;
    }
}
