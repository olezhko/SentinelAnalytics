using System.ComponentModel.DataAnnotations;

namespace SentinelAnalytics.Data.Entities;

public class PricingPlan
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    public required string Name { get; set; }
    public required string Description { get; set; }

    public decimal Price { get; set; }
    public int MaxProjects { get; set; }
    public int MaxEventsPerMonth { get; set; }
    public int MaxCrashesPerMonth { get; set; }
    public int MaxTeamMembersPerProject { get; set; }
    public string? StripePriceId { get; set; }
}