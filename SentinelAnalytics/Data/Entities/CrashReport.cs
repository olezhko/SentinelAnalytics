using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SentinelAnalytics.Data.Entities;

public class CrashReport
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    [Required]
    public string SessionId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Severity Severity { get; set; }

    [Required]
    public string ExceptionName { get; set; }
    public string Message { get; set; }
    public string StackTrace { get; set; }

    public string AppVersion { get; set; }
    public string OsVersion { get; set; }
    public string DeviceModel { get; set; }
    public string UserId { get; set; }

    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; }
}