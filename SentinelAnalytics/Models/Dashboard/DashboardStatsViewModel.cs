using SentinelAnalytics.Data.Entities;

namespace SentinelAnalytics.Models.Dashboard;

public class DashboardStatsViewModel
{
    public Guid CurrentProjectId { get; set; }
    public required string ProjectName { get; set; }
    public int TotalCrashes { get; set; }
    public int ActiveSessionsCount { get; set; }
    public int UniqueSessionsImpacted { get; set; }
    public double CrashFreeUserRate { get; set; }
    public List<DailyStat> DailyTrends { get; set; } = [];
    public List<CrashReport> RecentCrashes { get; set; } = [];
    public List<CrashReportSummary> GroupedCrashes { get; set; } = [];

    public List<string> AvailableVersions { get; set; } = new List<string>();
    public string? SelectedVersion { get; set; }
    public Severity? SelectedSeverity { get; set; }
    public string? SelectedPeriod { get; set; }
    public string? SelectedResolution { get; set; } // "all", "resolved", "unresolved"
    public string? SearchQuery { get; set; }
}

public record CrashReportGroup
{
    public required string ExceptionName { get; set; }
    public required string Message { get; set; }

    public required Guid[] Ids { get; set; } 
    public DateTimeOffset LastTriggered { get; set; }

    public bool IsResolved { get; set; } = false;
    public Severity Severity { get; set; }
}

public class DailyStat
{
    public required string Date { get; set; }
    public int Count { get; set; }
}