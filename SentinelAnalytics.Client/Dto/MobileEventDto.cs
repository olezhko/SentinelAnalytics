namespace SentinelAnalytics.MAUI.Dto;

internal sealed class MobileEventDto
{
    public required string EventName { get; init; }
    public required string SessionId { get; init; }
    public object? Properties { get; init; }
}