using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SentinelAnalytics.Data.Entities;

public class MobileEvent
{
    [Key]
    public long Id { get; set; }
    public Guid ProjectId { get; set; }
    public required string SessionId { get; set; }
    public required string EventName { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public string? PropertiesJson { get; set; }

    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; } = null!;
}