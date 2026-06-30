using System.Diagnostics.Metrics;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

var oTelCollectorEndpoint = new Uri("http://localhost:4317");
var oTelCollectorProtocol = OtlpExportProtocol.Grpc;
var serviceName = "Infinite_Gambler_OTel";
var resourceBuilder = ResourceBuilder.CreateDefault().AddService(serviceName);

var meterProvider = Sdk.CreateMeterProviderBuilder()
    .SetResourceBuilder(resourceBuilder)
    .AddMeter(serviceName)
    .AddOtlpExporter((exporterOptions, readerOptions) =>
    {
        exporterOptions.Endpoint = oTelCollectorEndpoint;
        exporterOptions.Protocol = oTelCollectorProtocol;
        readerOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 5000;
    })
    .Build();

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddOpenTelemetry(logging =>
    {
        logging.SetResourceBuilder(resourceBuilder);
        logging.IncludeFormattedMessage = true;
        logging.IncludeScopes = true;
        logging.AddOtlpExporter(options =>
        {
            options.Endpoint = oTelCollectorEndpoint;
            options.Protocol = oTelCollectorProtocol;
        });
    });
});

// =====

var logger = loggerFactory.CreateLogger(serviceName);
var meter = new Meter(serviceName);
var loopCounter = meter.CreateCounter<long>("example.loop.iterations", description: "Total iteration in main loop.");

long iteration = 0;
while (iteration++ < long.MaxValue)
{
    logger.LogInformation($"Loop iteration {iteration} started.");
    loopCounter.Add(1);
    await Task.Delay(100);
}

meterProvider.Dispose();
loggerFactory.Dispose();
