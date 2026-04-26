using SentinelAnalytics.Data.Entities;

namespace SentinelAnalytics.Models.Dashboard;

public class BillingViewModel
{
    public PricingPlan CurrentPlan { get; set; }
    public List<PricingPlan> AvailablePlans { get; set; }

    // Usage Metrics
    public int CurrentProjectCount { get; set; }
    public int CurrentEventCountMonth { get; set; }
    public int CurrentCrashCountMonth { get; set; }

    // Percentages for UI
    public double ProjectUsagePercent => CurrentPlan?.MaxProjects > 0 ? (double)CurrentProjectCount / CurrentPlan.MaxProjects * 100 : 0;
    public double EventUsagePercent => CurrentPlan?.MaxEventsPerMonth > 0 ? (double)CurrentEventCountMonth / CurrentPlan.MaxEventsPerMonth * 100 : 0;
    public double CrashUsagePercent => CurrentPlan?.MaxCrashesPerMonth > 0 ? (double)CurrentCrashCountMonth / CurrentPlan.MaxCrashesPerMonth * 100 : 0;
}