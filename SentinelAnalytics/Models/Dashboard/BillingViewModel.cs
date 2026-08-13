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

    // Subscription
    public DateTime StartDate { get; set; }
    public string? StripeSubscriptionStatus { get; set; }

    // Payment method (last 4 only — full card number never stored)
    public string? CardLastFour { get; set; }
    public string? CardExpiry { get; set; }
    public string? CardBrand { get; set; }
    public string? CardholderName { get; set; }

    public bool HasPaymentMethod => CardLastFour != null;

    public DateTime NextBillingDate
    {
        get
        {
            var day = StartDate.Day;
            var now = DateTime.UtcNow;
            var candidate = new DateTime(now.Year, now.Month, Math.Min(day, DateTime.DaysInMonth(now.Year, now.Month)));
            if (candidate <= now)
            {
                var next = now.AddMonths(1);
                candidate = new DateTime(next.Year, next.Month, Math.Min(day, DateTime.DaysInMonth(next.Year, next.Month)));
            }
            return candidate;
        }
    }

    // Percentages for UI
    public double ProjectUsagePercent => CurrentPlan?.MaxProjects > 0 ? (double)CurrentProjectCount / CurrentPlan.MaxProjects * 100 : 0;
    public double EventUsagePercent => CurrentPlan?.MaxEventsPerMonth > 0 ? (double)CurrentEventCountMonth / CurrentPlan.MaxEventsPerMonth * 100 : 0;
    public double CrashUsagePercent => CurrentPlan?.MaxCrashesPerMonth > 0 ? (double)CurrentCrashCountMonth / CurrentPlan.MaxCrashesPerMonth * 100 : 0;
}