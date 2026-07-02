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
    private readonly int _indexOfCheapestCasino;
    private readonly decimal _bettingCostOfCheapestGame;

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

        _indexOfCheapestCasino = _casinos
            .Select((casino, index) => (casino, index))
            .OrderBy(x => x.casino.PurchasePrice)
            .First()
            .index;

        _bettingCostOfCheapestGame = _casinos.SelectMany(c => c.Games).Min(g => g.BetCost);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Simulation started.");
            _metrics.SimulationStarted();
            var sw = Stopwatch.StartNew();

            ShowPlayersRanking();

            while (!CanSomePlayerBuyACasino() && CanSomePlayerBet())
            {
                RunGamblingRound();
            }

            if (CanSomePlayerBuyACasino())
                ShowMessageForCasinoPurchased();

            if (!CanSomePlayerBet())
                ShowMessageForAllPlayersBankrupt();

            ShowPlayersRanking();

            _metrics.SimulationCompleted(sw.Elapsed.TotalMilliseconds);
            _logger.LogInformation("Simulation ended.");
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
            var topSummary = GetAffordableGameSummaries(player)
                .OrderByDescending(s => s.ReturnOnInvestment * (decimal)s.Odds)
                .First();

            _casinos[topSummary.CasinoIndex].PlayGame(topSummary.GameIndex, player);
            _metrics.SpinsPlayed(1);
        }
    }

    private AffordableGameSummary[] GetAffordableGameSummaries(Player player)
    {
        return _casinos
            .SelectMany(
                (casino, casinoIndex) =>
                    casino
                        .GetIndexesOfGamesWithinBudget(player.CashBalance)
                        .Select(gameIndex =>
                        {
                            var game = casino.Games[gameIndex];
                            return new AffordableGameSummary(
                                casinoIndex,
                                gameIndex,
                                game.Odds,
                                (game.Payout - game.BetCost) / game.BetCost
                            );
                        })
                        .ToArray()
            )
            .ToArray();
    }
}
