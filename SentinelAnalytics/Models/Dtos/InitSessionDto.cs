namespace SentinelAnalytics.Models.Dtos
{
    public sealed class InitSessionDto
    {
        public required string DeviceId { get; set; }
        public required string Country { get; set; }
        public required string Language { get; set; }

        public required string AppVersion { get; set; }
        public required string OsVersion { get; set; }
        public required string DeviceModel { get; set; }
    }
}
