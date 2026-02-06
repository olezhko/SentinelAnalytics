namespace SentinelAnalytics.Client;

public sealed class SentinelOptions
{
    public required string ApiKey { get; init; }
    public required string Endpoint { get; init; }
    public string AppVersion { get; init; } = "unknown";
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(10);
}