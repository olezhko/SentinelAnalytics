namespace SentinelAnalytics.MAUI.Dto;

internal sealed class CrashReportDto
{
    public required string SessionId { get; init; }
    public string Severity { get; init; } = "Error";
    public required string ExceptionName { get; init; }
    public required string Message { get; init; }
    public required string StackTrace { get; init; }
    public string? UserId { get; init; }
    public IDictionary<string, object>? Properties { get; init; }
}