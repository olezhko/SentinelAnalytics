namespace SentinelAnalytics.Models.Dtos
{
    public sealed class MobileEventDto
    {
        public required string SessionId { get; set; }
        public required string EventName { get; set; }
        public required Dictionary<string, string> Properties { get; set; }
    }
}
