namespace SentinelAnalytics.Client;

public sealed class CrashReportDto
{
    public required string SessionId { get; init; }
    public required string ExceptionName { get; init; }
    public required string Message { get; init; }
    public required string StackTrace { get; init; }
    public required string AppVersion { get; init; }
    public required string OsVersion { get; init; }
    public required string DeviceModel { get; init; }
    public string Severity { get; init; } = "Error";
    public string? UserId { get; init; }
}