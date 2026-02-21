namespace SentinelAnalytics.Models.Analytics;

public class AnalyticsViewModel
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; }
    public string SelectedVersion { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public List<string> AvailableVersions { get; set; } = new List<string>();

    // 1. Active Users per day
    public List<ChartDataPoint> ActiveUsersPerDay { get; set; } = new List<ChartDataPoint>();

    // 2. Session Duration Distribution
    public List<ChartDataPoint> SessionDurations { get; set; } = new List<ChartDataPoint>();

    // 3. Country Distribution
    public List<ChartDataPoint> RegionalStats { get; set; } = new List<ChartDataPoint>();

    // 4. Language Distribution
    public List<ChartDataPoint> LanguageStats { get; set; } = new List<ChartDataPoint>();
}

public class ChartDataPoint
{
    public string Label { get; set; }
    public double Value { get; set; }
}