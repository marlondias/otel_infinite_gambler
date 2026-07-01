using System.Diagnostics.Metrics;
using InfiniteGambler.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

// Load configuration from appsettings.json
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var oTelConfig = configuration.GetRequiredSection("OpenTelemetry").Get<OpenTelemetryConfig>();
if (oTelConfig is null)
    throw new InvalidOperationException("OpenTelemetry configuration is invalid.");

// Set reusable values for setting up OpenTelemetry
var oTelResourceBuilder = ResourceBuilder.CreateDefault().AddService(oTelConfig.ServiceName);
var oTelCollectorUri = new Uri(oTelConfig.Collector.Endpoint);
var oTelCollectorProtocol = oTelConfig.Collector.UseGrpc
    ? OtlpExportProtocol.Grpc
    : OtlpExportProtocol.HttpProtobuf;

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddOpenTelemetry(logging =>
    {
        logging.SetResourceBuilder(oTelResourceBuilder);
        logging.IncludeFormattedMessage = true;
        logging.IncludeScopes = true;
        logging.AddOtlpExporter(options =>
        {
            options.Protocol = oTelCollectorProtocol;
            options.Endpoint = oTelCollectorUri;
        });
    });
});

var meterProvider = Sdk.CreateMeterProviderBuilder()
    .SetResourceBuilder(oTelResourceBuilder)
    .AddOtlpExporter(
        (exporterOptions, readerOptions) =>
        {
            exporterOptions.Protocol = oTelCollectorProtocol;
            exporterOptions.Endpoint = oTelCollectorUri;
            readerOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds =
                oTelConfig.MetricsExportIntervalInMS;
        }
    )
    .AddMeter(oTelConfig.ServiceName)
    .Build();

// =====

var logger = loggerFactory.CreateLogger(oTelConfig.ServiceName);

var meter = new Meter(oTelConfig.ServiceName);
var loopCounter = meter.CreateCounter<long>(
    "example.loop.iterations",
    description: "Total iteration in main loop."
);

long iteration = 0;
while (iteration++ < long.MaxValue)
{
    logger.LogInformation($"Loop iteration {iteration} started.");
    loopCounter.Add(1);
    await Task.Delay(100);
}

meterProvider.Dispose();
loggerFactory.Dispose();

// var appConfig = configuration.GetSection("Application").Get<ApplicationConfig>();
