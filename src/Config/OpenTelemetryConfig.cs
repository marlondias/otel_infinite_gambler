namespace InfiniteGambler.Config;

public record OpenTelemetryCollectorConfig(string Endpoint, bool UseGrpc);

public record OpenTelemetryConfig(
    string ServiceName,
    int MetricsExportIntervalInMS,
    OpenTelemetryCollectorConfig Collector
);
