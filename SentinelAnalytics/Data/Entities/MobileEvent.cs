using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SentinelAnalytics.Data.Entities;

public class MobileEvent
{
    [Key]
    public long Id { get; set; }
    public Guid ProjectId { get; set; }
    [Required]
    public string SessionId { get; set; }
    [Required]
    public string EventName { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Stored as JSON string in MSSQL for flexibility
    public string PropertiesJson { get; set; }

    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; }
}