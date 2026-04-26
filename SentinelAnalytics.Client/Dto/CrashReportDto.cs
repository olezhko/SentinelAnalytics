using System.Text.Json.Serialization;

namespace SentinelAnalytics.MAUI.Dto;

internal sealed class CrashReportDto
{
    public required string SessionId { get; init; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Severity Severity { get; set; }
    public required string ExceptionName { get; init; }
    public required string Message { get; init; }
    public required string StackTrace { get; init; }
    public string? UserId { get; init; }
    public IDictionary<string, object>? Properties { get; init; }
}