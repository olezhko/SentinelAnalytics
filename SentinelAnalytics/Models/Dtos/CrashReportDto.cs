using SentinelAnalytics.Data.Entities;
using System.Text.Json.Serialization;

namespace SentinelAnalytics.Models.Dtos
{
    public sealed class CrashReportDto
    {
        public required Guid SessionId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Severity Severity { get; set; }

        public required string ExceptionName { get; set; }
        public required string Message { get; set; }
        public required string StackTrace { get; set; }

        public string? UserId { get; set; }

        public Dictionary<string, string>? Properties { get; set; }
    }
}
