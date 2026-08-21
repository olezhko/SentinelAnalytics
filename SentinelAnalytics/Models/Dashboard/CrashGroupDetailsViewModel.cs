using SentinelAnalytics.Data.Entities;

namespace SentinelAnalytics.Models.Dashboard;

public sealed class CrashGroupDetailsViewModel
{
    public required List<CrashReport> Crashes { get; set; }
    public Guid ProjectId { get; set; }

    public string? AppVersion { get; set; }
    public string? TimePeriod { get; set; }
    public Severity? Severity { get; set; }
    public string? ResolutionStatus { get; set; }
    public string? SearchQuery { get; set; }
}
