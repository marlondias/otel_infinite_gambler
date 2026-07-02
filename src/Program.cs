using InfiniteGambler;
using InfiniteGambler.Config;
using InfiniteGambler.Factories;
using InfiniteGambler.Instrumentation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<ApplicationConfig>(
    builder.Configuration.GetRequiredSection("Application")
);

var oTelConfig =
    builder.Configuration.GetRequiredSection("OpenTelemetry").Get<OpenTelemetryConfig>()
    ?? throw new InvalidOperationException("OpenTelemetry configuration is invalid.");

var oTelCollectorUri = new Uri(oTelConfig.Collector.Endpoint);
var oTelCollectorProtocol = oTelConfig.Collector.UseGrpc
    ? OtlpExportProtocol.Grpc
    : OtlpExportProtocol.HttpProtobuf;

builder
    .Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService(oTelConfig.ServiceName))
    .WithLogging(logging =>
    {
        //logging.IncludeFormattedMessage = true;
        //logging.IncludeScopes = true;
        logging.AddOtlpExporter(o =>
        {
            o.Protocol = oTelCollectorProtocol;
            o.Endpoint = oTelCollectorUri;
        });
    })
    .WithMetrics(metrics =>
    {
        metrics.AddMeter(Instrumentation.MeterName);
        metrics.AddOtlpExporter(o =>
        {
            o.Protocol = oTelCollectorProtocol;
            o.Endpoint = oTelCollectorUri;
        });
    });

builder.Services.AddSingleton<Bogus.Faker>();
builder.Services.AddSingleton<SimulationMetrics>();

builder.Services.AddTransient<PlayerFactory>();
builder.Services.AddTransient<GameFactory>();
builder.Services.AddTransient<CasinoFactory>();
builder.Services.AddTransient<SimulationLauncher>();

using var host = builder.Build();

var simulation = host.Services.GetRequiredService<SimulationLauncher>();
simulation.Run();

Console.WriteLine("Simulation completed. Check Grafana for details.");
