using SentinelAnalytics.Data.Entities;

namespace SentinelAnalytics.Models;

public class DashboardStatsViewModel
{
    public Guid CurrentProjectId { get; set; }
    public required string ProjectName { get; set; }
    public int TotalCrashes { get; set; }
    public int ActiveSessionsCount { get; set; }
    public List<DailyStat> DailyTrends { get; set; } = [];
    public List<CrashReport> RecentCrashes { get; set; } = [];

    public List<string> AvailableVersions { get; set; } = new List<string>();
    public string? SelectedVersion { get; set; }
    public Severity? SelectedSeverity { get; set; }
    public string? SelectedPeriod { get; set; }
}

public class DailyStat
{
    public required string Date { get; set; }
    public int Count { get; set; }
}