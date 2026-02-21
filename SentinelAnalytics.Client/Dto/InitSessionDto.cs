namespace SentinelAnalytics.MAUI.Dto
{
    internal sealed class InitSessionDto
    {
        public required string DeviceId { get; set; }
        public required string Country { get; set; }
        public required string Language { get; set; }
        public required string AppVersion { get; init; }
        public required string OsVersion { get; init; }
        public required string DeviceModel { get; init; }
    }
}
