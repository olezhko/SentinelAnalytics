using SentinelAnalytics.Data.Entities;
using System.Text.Json.Serialization;

namespace SentinelAnalytics.Models.Dtos
{
    public sealed class CrashReportDto
    {
        public required string SessionId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Severity Severity { get; set; }

        public required string ExceptionName { get; set; }
        public required string Message { get; set; }
        public required string StackTrace { get; set; }

        public required string AppVersion { get; set; }
        public required string OsVersion { get; set; }
        public required string DeviceModel { get; set; }
        public string? UserId { get; set; }

        public Dictionary<string, string>? Properties { get; set; }
    }
}
