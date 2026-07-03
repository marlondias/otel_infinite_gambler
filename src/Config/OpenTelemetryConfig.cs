namespace InfiniteGambler.Config;

public sealed class OpenTelemetryCollectorConfig
{
    public required string Endpoint { get; init; }
    public required bool UseGrpc { get; init; }
}

public sealed class OpenTelemetryConfig
{
    public required string ServiceName { get; init; }
    public required string ServiceInstanceId { get; init; }
    public required int MetricsExportIntervalInMS { get; init; }
    public required OpenTelemetryCollectorConfig Collector { get; init; }
}
