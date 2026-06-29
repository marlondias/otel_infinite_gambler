using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

var oTelCollectorEndpoint = new Uri("http://localhost:4318");
var oTelCollectorProtocol = OtlpExportProtocol.HttpProtobuf;

var meterProvider = Sdk.CreateMeterProviderBuilder()
    // Other setup code, like setting a resource goes here too
    .AddOtlpExporter(options =>
    {
        options.Endpoint = oTelCollectorEndpoint;
        options.Protocol = oTelCollectorProtocol;
    })
    .Build();

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddOpenTelemetry(logging =>
    {
        logging.AddOtlpExporter(options =>
        {
            options.Endpoint = oTelCollectorEndpoint;
            options.Protocol = oTelCollectorProtocol;
        });
    });
});

Console.WriteLine("Hello, World!");
