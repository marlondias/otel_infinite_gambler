using Bogus;
using InfiniteGambler;
using InfiniteGambler.Config;
using InfiniteGambler.Factories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// Set reusable values for setting up OpenTelemetry
var oTelConfig = configuration.GetRequiredSection("OpenTelemetry").Get<OpenTelemetryConfig>();
if (oTelConfig is null)
{
    throw new InvalidOperationException("OpenTelemetry configuration is invalid.");
}
var oTelResourceBuilder = ResourceBuilder.CreateDefault().AddService(oTelConfig.ServiceName);
var oTelCollectorUri = new Uri(oTelConfig.Collector.Endpoint);
var oTelCollectorProtocol = oTelConfig.Collector.UseGrpc
    ? OtlpExportProtocol.Grpc
    : OtlpExportProtocol.HttpProtobuf;

var services = new ServiceCollection();
services.Configure<ApplicationConfig>(configuration.GetRequiredSection("Application"));

services.AddLogging(builder =>
{
    builder.AddOpenTelemetry(logging =>
    {
        logging.IncludeFormattedMessage = true;
        logging.IncludeScopes = true;
        logging.SetResourceBuilder(oTelResourceBuilder);
        logging.AddOtlpExporter(options =>
        {
            options.Protocol = oTelCollectorProtocol;
            options.Endpoint = oTelCollectorUri;
        });
    });
});

services.AddSingleton<Faker>();
services.AddTransient<PlayerFactory>();
services.AddTransient<GameFactory>();
services.AddTransient<CasinoFactory>();
services.AddTransient<SimulationLauncher>();

using var serviceProvider = services.BuildServiceProvider(
    new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true }
);

var simulation = serviceProvider.GetRequiredService<SimulationLauncher>();
simulation.Run();

Console.WriteLine("Program finished. Check Grafana for details.");
