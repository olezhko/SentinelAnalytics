using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SentinelAnalytics.Data.Entities;

public class CrashReport
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Severity Severity { get; set; }

    public required string ExceptionName { get; set; }
    public required string Message { get; set; }
    public required string StackTrace { get; set; }
    public string? UserId { get; set; }
    public string? PropertiesJson { get; set; }


    // Resolution Fields
    public bool IsResolved { get; set; } = false;
    public DateTime? ResolvedAt { get; set; }
    public string? ResolutionComment { get; set; }
    public bool IsIgnored { get; set; } = false;

    public Guid ProjectId { get; set; }
    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; } = null!;

    public required Guid SessionId { get; set; }
    [ForeignKey("SessionId")]
    public virtual Session Session { get; set; } = null!;
}