namespace SentinelAnalytics.Models.Dtos
{
    public sealed class MobileEventDto
    {
        public required Guid SessionId { get; set; }
        public required string EventName { get; set; }
        public Dictionary<string, string>? Properties { get; set; }
    }
}
