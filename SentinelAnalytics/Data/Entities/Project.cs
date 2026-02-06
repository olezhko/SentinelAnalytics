using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SentinelAnalytics.Data.Entities;

public class Project
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    public string Name { get; set; }
    [Required]
    public string ApiKey { get; set; } = Guid.NewGuid().ToString("N");
    public required string Platform { get; set; } // iOS, Android, Cross-Platform
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public virtual ICollection<CrashReport> Crashes { get; set; }
    public virtual ICollection<MobileEvent> Events { get; set; }

    public required string UserId { get; set; }

    [ForeignKey("UserId")]
    public virtual IdentityUser User { get; set; } = null!;
}