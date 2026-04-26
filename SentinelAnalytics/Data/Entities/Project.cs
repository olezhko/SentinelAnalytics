using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SentinelAnalytics.Data.Entities;

public enum PlanType { Free, Pro }

public class Project : AuditableEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public string ApiKey { get; set; } = Guid.NewGuid().ToString("N");
    public required string Platform { get; set; } // iOS, Android, Cross-Platform
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public PlanType Plan { get; set; } = PlanType.Free;
    public virtual ICollection<CrashReport> Crashes { get; set; } = [];
    public virtual ICollection<MobileEvent> Events { get; set; } = [];
    public virtual ICollection<ProjectMember> Members { get; set; } = [];

    public required string UserId { get; set; }

    [ForeignKey("UserId")]
    public virtual IdentityUser User { get; set; } = null!;
}