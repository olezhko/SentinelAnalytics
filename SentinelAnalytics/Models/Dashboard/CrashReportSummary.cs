using SentinelAnalytics.Data.Entities;

namespace SentinelAnalytics.Models.Dashboard;

public sealed class CrashReportSummary
{
    public IReadOnlyCollection<CrashReport> Reports { get; set; } = [];
    public int OccurrenceCount { get; set; }
    public int AffectedUsersCount { get; set; }
    public bool IsRegression { get; set; }
    public bool AllResolved { get; set; }
    public bool AllIgnored { get; set; }

    public required string Message { get; set; }
    public DateTime LastCrashAt { get; set; }

    public required ExceptionStackTrace Exception { get; set; }
}