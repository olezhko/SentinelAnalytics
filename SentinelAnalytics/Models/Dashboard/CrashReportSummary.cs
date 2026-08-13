using SentinelAnalytics.Data.Entities;

namespace SentinelAnalytics.Models.Dashboard;

public sealed class CrashReportSummary
{
    public CrashReport Report { get; set; } = null!;
    public int OccurrenceCount { get; set; }
    public int AffectedUsersCount { get; set; }
    public bool IsRegression { get; set; }

    public required ExceptionStackTrace Exception { get; set; }
}