using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SentinelAnalytics.Data.Entities;

public class UserSubscription : AuditableEntity
{
    [Key]
    public Guid Id { get; set; }
    public required string UserId { get; set; }

    public bool NotifyOnCritical { get; set; } = true;
    public bool NotifyOnError { get; set; } = true;
    public bool NotifyOnRegression { get; set; } = true;

    [ForeignKey("UserId")]
    public virtual IdentityUser User { get; set; } = null!;
}
