using SentinelAnalytics.Data.Entities;

namespace SentinelAnalytics.Models;

public class DashboardStatsViewModel
{
    public required string ProjectName { get; set; }
    public int TotalCrashes { get; set; }
    public int ActiveUsersCount { get; set; }
    public List<DailyStat> DailyTrends { get; set; }
    public List<CrashReport> RecentCrashes { get; set; }
    public Dictionary<string, int> DeviceDistribution { get; set; }
}

public class DailyStat
{
    public required string Date { get; set; }
    public int Count { get; set; }
}